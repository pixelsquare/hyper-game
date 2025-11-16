using System;
using System.Collections.Generic;
using System.Linq;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public struct UserProfileForSocialScreen
    {
        public UserProfile UserProfile;
        public SocialState SocialState;

        /// <summary>
        /// The state of the social screen
        /// </summary>
        public SocialScreen.PanelState PanelState;

        public Action OnActionMadeCallback;
        public Action<bool> ShowSocialsLoading;
        public string RoomId;

        public bool IsSpacer;
        public string SpacerLabel;
    }

    public class SocialScreen : MonoBehaviour
    {
        [SerializeField] private Button searchButton;
        [SerializeField] private Button exitSearchButton;
        [SerializeField] private TMP_InputField searchInputField;
        [SerializeField] private Transform searchContentPanel;
        [SerializeField] private GameObject searchContentLoading;
        [SerializeField] private TMP_Text searchHintText;
        [SerializeField] private UserPanelOSAList searchUserOsaList;
        [SerializeField] private Transform normalContentPanel;
        [SerializeField] private UserPanelOSAList userOsaList;
        [SerializeField] private GameObject loadingIndicator;
        [SerializeField] private RectTransform searchContainer;
        [SerializeField] private LayoutElement headerLayoutElement;
        [SerializeField] private RectTransform mainRect;
        [SerializeField] private Transform searchToggleContainer;
        [SerializeField] private Toggle friendsToggle;
        [SerializeField] private Toggle followersToggle;
        [SerializeField] private Toggle followingToggle;
        [SerializeField] private Toggle unrelatedToggle;
        [SerializeField] private UnityEvent followerListRefresh;
        [SerializeField] private UnityEvent followingListRefresh;
        [SerializeField] private UnityEvent friendListRefresh;

        private readonly List<UserProfileForSocialScreen> searchResultsList = new();
        private PanelState currentPanelSelected = PanelState.Followers;
        private UnityEvent searchListRefresh = new();
        private bool isSearchActive;
        private Subject<string> searchInput = new();
        private HashSet<string> searchFilterHashSet = new();
        private Dictionary<Toggle, string> searchToggleKeyMap = new();
        private string currentSearchTerm = string.Empty;
        private float originalSearchContainerHeight;
        private float originalHeaderMinHeight;
        private float safeAreaHeightDelta;

        private GetSocialRelationshipsRequest latestSocialRelationshipsRequest;
        
        private Dictionary<PanelState, int> latestPageRequested = new()
        {
            {PanelState.Followers, 1}, 
            {PanelState.Following, 1}, 
            {PanelState.Friends, 1}, 
        };

        private Dictionary<PanelState, bool> hasNextPage = new()
        {
            {PanelState.Followers, true},
            {PanelState.Following, true},
            {PanelState.Friends, true},
        };

        public enum PanelState
        {
            Followers,
            Following,
            Friends,
            Search
        }

        public void Awake()
        {
            searchHintText.text = "Use the input field to search and make sure a check box is selected. Minimum of 2 characters.";
            originalSearchContainerHeight = searchContainer.sizeDelta.y;
            originalHeaderMinHeight = headerLayoutElement.minHeight;
            var scalingFactor = mainRect.lossyScale.x;
            safeAreaHeightDelta = Mathf.Abs(mainRect.rect.height * scalingFactor - (Screen.safeArea.height + Screen.safeArea.y)) / scalingFactor;
            searchInput.Throttle(TimeSpan.FromSeconds(1f)).Subscribe(ProcessSearchAsync).AddTo(this);

            Services.SocialService.OnSocialActionMade.Subscribe(t =>
            {
                GetSocialRelationshipsAsync(currentPanelSelected, hardReset: true);
                RefreshOwnUserProfileAsync();
            }).AddTo(this);

            searchToggleKeyMap = new Dictionary<Toggle, string>()
            {
                {friendsToggle, "friends"},
                {followingToggle, "following"},
                {followersToggle, "followers"},
                {unrelatedToggle, "unrelated"},
            };
            
            foreach (var (toggle, key) in searchToggleKeyMap)
            {
                searchFilterHashSet.Add(key);
                toggle.onValueChanged.AsObservable().Subscribe(_ =>
                {
                    searchContentLoading.SetActive(true);
                    searchHintText.transform.parent.gameObject.SetActive(false);
                    
                    if (toggle.isOn)
                    {
                        searchFilterHashSet.Add(key);
                    }
                    else
                    {
                        searchFilterHashSet.Remove(key);
                    }

                    ProcessSearchAsync(currentSearchTerm);
                }).AddTo(this);
            }
            
            // Logic that allows pagination to happen when reaching the end of the list
            // This uses normalized position
            userOsaList.ScrollPositionChanged += d =>
            {
                if (d != 0f) return;
                if (loadingIndicator.gameObject.activeInHierarchy) return;
                GetSocialRelationshipsAsync(currentPanelSelected, latestPageRequested[currentPanelSelected] + 1);
            };
        }

        public void OnEnable()
        {
            searchButton.onClick.AddListener(OnSearchButtonClick);
            searchInputField.onValueChanged.AddListener(OnInputFieldUpdate);
            exitSearchButton.onClick.AddListener(OnExitSearchButtonClick);
            searchListRefresh.AddListener(() => ProcessSearchAsync(currentSearchTerm));
        }

        public void OnDisable()
        {
            searchButton.onClick.RemoveListener(OnSearchButtonClick);
            searchInputField.onValueChanged.RemoveListener(OnInputFieldUpdate);
            exitSearchButton.onClick.RemoveListener(OnExitSearchButtonClick);
            searchListRefresh.RemoveListener(() => ProcessSearchAsync(currentSearchTerm));
        }

        /// <remarks>
        /// This is called via visual scripting
        /// </remarks>
        public void OnTabSelected(PanelState state)
        {
            GetSocialRelationshipsAsync(state, hardReset:true);
        }

        public async void GetSocialRelationshipsAsync(PanelState state, int pageToRequest = 1, bool hardReset = false)
        {
            if (!hardReset)
            {
                if (latestPageRequested[state] == pageToRequest || latestPageRequested[state] + 1 != pageToRequest)
                {
                    return;
                }

                if (!hasNextPage[state])
                {
                    return;
                }
            }
            else
            {
                hasNextPage[state] = true;
            }

            latestPageRequested[state] = pageToRequest;

            var userConstraint = state switch
            {
                PanelState.Followers => "followers",
                PanelState.Following => "following",
                PanelState.Friends => "friends",
                _ => ""
            };
            
            loadingIndicator.gameObject.SetActive(true);

            var localRecordedRequest = new GetSocialRelationshipsRequest
            {
                userConstraint = userConstraint,
                pageToRequest = pageToRequest,
            };

            if (latestSocialRelationshipsRequest == localRecordedRequest)
            {
                return;
            }
            
            latestSocialRelationshipsRequest = localRecordedRequest;

            var socialResults = await Services.SocialService.GetSocialRelationshipsAsync(localRecordedRequest);

            if (latestSocialRelationshipsRequest.pageToRequest != localRecordedRequest.pageToRequest ||
                latestSocialRelationshipsRequest.userConstraint != localRecordedRequest.userConstraint)
            {
                return;
            }

            if (latestPageRequested == null)
            {
                "latestPageRequested is null!".LogError();
                loadingIndicator.gameObject.SetActive(false);
                return;
            }

            if (socialResults == null)
            {
                "socialResults is null!".LogError();
                loadingIndicator.gameObject.SetActive(false);
                return;
            }

            if (socialResults.Result == null)
            {
                "socialResults.Result is null".LogError();
                loadingIndicator.gameObject.SetActive(false);
                return;
            }
            
            latestPageRequested[state] = socialResults.Result.CurrentPage;
            hasNextPage[state] = socialResults.Result.HasNextPage;
            
            switch (state)
            {
                case PanelState.Followers:
                    PopulateFollowersPanels(socialResults.Result);
                    break;
                case PanelState.Following:
                    PopulateFollowingPanels(socialResults.Result);
                    break;
                case PanelState.Friends:
                    PopulateFriendsPanel(socialResults.Result);
                    break;
            }
            
            loadingIndicator.gameObject.SetActive(false);

            if (hardReset && userOsaList.Data.Count > 0)
            {
                userOsaList.ScrollTo(0);
            }
        }

        public void PopulateFollowersPanels(GetSocialRelationshipsResult result)
        {
            var isFreshPage = result.CurrentPage == 1;
            currentPanelSelected = PanelState.Followers;
            var tempList = new List<UserProfileForSocialScreen>();
            if (result.OtherPlayerProfiles is { } followers)
            {
                tempList.AddRange(followers.Select(t => new UserProfileForSocialScreen
                {
                    UserProfile = t.profile,
                    SocialState = t.social_state,
                    OnActionMadeCallback = () =>
                    {
                    },
                    RoomId = t.room_id,
                    ShowSocialsLoading = b => loadingIndicator.SetActive(b),
                    PanelState = PanelState.Followers,
                }));
            }

            if (isFreshPage)
            {
                userOsaList.SetItems(tempList);
            }
            else
            {
                userOsaList.AddItemsAt(userOsaList.GetItemsCount(), tempList);
            }
        }

        public void PopulateFollowingPanels(GetSocialRelationshipsResult result)
        {
            var isFreshPage = result.CurrentPage == 1;
            currentPanelSelected = PanelState.Following;
            var tempList = new List<UserProfileForSocialScreen>();
            if (result.OtherPlayerProfiles is { } following)
            {
                tempList.AddRange(following.Select(t => new UserProfileForSocialScreen
                {
                    UserProfile = t.profile,
                    SocialState = t.social_state,
                    OnActionMadeCallback = () =>
                    {
                    },
                    RoomId = t.room_id,
                    ShowSocialsLoading = b => loadingIndicator.SetActive(b),
                    PanelState = PanelState.Following,
                }));
            }
            
            if (isFreshPage)
            {
                userOsaList.SetItems(tempList);
            }
            else
            {
                userOsaList.AddItemsAt(userOsaList.GetItemsCount(), tempList);
            }
        }

        public void PopulateFriendsPanel(GetSocialRelationshipsResult result)
        {
            var isFreshPage = result.CurrentPage == 1;
            currentPanelSelected = PanelState.Friends;
            var tempList = new List<UserProfileForSocialScreen>();
            if (result.OtherPlayerProfiles is { } friends)
            {
                tempList.AddRange(friends.Select(t => new UserProfileForSocialScreen
                {
                    UserProfile = t.profile,
                    SocialState = t.social_state,
                    OnActionMadeCallback = () =>
                    {
                    },
                    ShowSocialsLoading = b => loadingIndicator.SetActive(b),
                    RoomId = t.room_id,
                    PanelState = PanelState.Friends,
                }));
            }
            
            if (isFreshPage)
            {
                userOsaList.SetItems(tempList);
            }
            else
            {
                userOsaList.AddItemsAt(userOsaList.GetItemsCount(), tempList);
            }
        }

        private void PopulateSearchPanel(FindUserResult result)
        {
            searchResultsList.Clear();
            var tempList = new List<UserProfileForSocialScreen>();
            
            var friends = result.Friends?.ToList();
            var following = result.Following?.ToList();
            var followers = result.Followers?.ToList();
            var unrelated = result.Unrelated?.ToList();
            if (friends is {Count: > 0})
            {
                // add "spacer" data
                tempList.Add(new UserProfileForSocialScreen
                {
                    IsSpacer = true,
                    SpacerLabel = "Friends",
                });
                tempList.AddRange(friends.Select(t => new UserProfileForSocialScreen
                {
                    UserProfile = t.profile,
                    SocialState = t.social_state,
                    OnActionMadeCallback = () =>
                    {
                        searchListRefresh.Invoke();
                        RefreshOwnUserProfileAsync();
                    },
                    ShowSocialsLoading = b => searchContentLoading.SetActive(b),
                    PanelState = PanelState.Friends,
                }).OrderByDescending(t => t.SocialState.HasFlag(SocialState.Favorite)));
            }

            if (following is { Count: > 0 })
            {
                // add "spacer" data
                tempList.Add(new UserProfileForSocialScreen
                {
                    IsSpacer = true,
                    SpacerLabel = "Following",
                });
                tempList.AddRange(following.Select(t => new UserProfileForSocialScreen
                {
                    UserProfile = t.profile,
                    SocialState = t.social_state,
                    OnActionMadeCallback = () =>
                    {
                        searchListRefresh.Invoke();
                        RefreshOwnUserProfileAsync();
                    },
                    ShowSocialsLoading = b => searchContentLoading.SetActive(b),
                    PanelState = PanelState.Following,
                }).OrderByDescending(t => t.SocialState.HasFlag(SocialState.Favorite)));
            }

            if (followers is { Count: > 0 })
            {
                // add "spacer" data
                tempList.Add(new UserProfileForSocialScreen
                {
                    IsSpacer = true,
                    SpacerLabel = "Followers",
                });
                tempList.AddRange(followers.Select(t => new UserProfileForSocialScreen
                {
                    UserProfile = t.profile,
                    SocialState = t.social_state,
                    OnActionMadeCallback = () =>
                    {
                        searchListRefresh.Invoke();
                        RefreshOwnUserProfileAsync();
                    },
                    ShowSocialsLoading = b => searchContentLoading.SetActive(b),
                    PanelState = PanelState.Followers,
                }).OrderByDescending(t => t.SocialState.HasFlag(SocialState.Favorite)));
            }

            if (unrelated is { Count: > 0 })
            {
                // add "spacer" data
                tempList.Add(new UserProfileForSocialScreen
                {
                    IsSpacer = true,
                    SpacerLabel = "Unrelated/New",
                });
                tempList.AddRange(unrelated.Select(t => new UserProfileForSocialScreen
                {
                    UserProfile = t.profile,
                    SocialState = t.social_state,
                    OnActionMadeCallback = () =>
                    {
                        searchListRefresh.Invoke();
                        RefreshOwnUserProfileAsync();
                    },
                    ShowSocialsLoading = b => searchContentLoading.SetActive(b),
                    PanelState = PanelState.Search,
                }).OrderByDescending(t => t.SocialState.HasFlag(SocialState.Favorite)));
            }

            searchResultsList.AddRange(tempList);

            if (searchResultsList.Count == 0)
            {
                searchHintText.text = "No user found";
                searchHintText.transform.parent.gameObject.SetActive(true);
            }

            searchUserOsaList.SetItems(searchResultsList.ToList());
        }

        private void OnSearchButtonClick()
        {
            isSearchActive = true;
            UpdateSearchUI();
        }

        private void OnExitSearchButtonClick()
        {
            if (!isSearchActive)
            {
                SceneLoadingManager.Instance.UnloadScene(SceneNames.SOCIAL_SCREEN, () =>
                {
                    SceneLoadingManager.Instance.SetActiveScene(SceneNames.USER_PROFILE_SCREEN);
                });
                return;
            }

            isSearchActive = false;
            UpdateSearchUI();

            switch (currentPanelSelected)
            {
                case PanelState.Followers:
                    followerListRefresh?.Invoke();
                    break;

                case PanelState.Following:
                    followingListRefresh?.Invoke();
                    break;

                case PanelState.Friends:
                    friendListRefresh?.Invoke();
                    break;
            }
        }

        private void OnInputFieldUpdate(string searchTerm)
        {
            searchContentLoading.SetActive(true);
            searchHintText.transform.parent.gameObject.SetActive(false);
            searchInput.OnNext(searchTerm);
        }

        private async void ProcessSearchAsync(string searchTerm)
        {
            currentSearchTerm = searchTerm;
            if (searchTerm is null or "" || !searchFilterHashSet.Any() || searchTerm.Length <= 1)
            {
                PopulateSearchPanel(new FindUserResult());
                searchHintText.text = "Use the input field to search and make sure a check box is selected. Minimum of 2 characters.";
                searchHintText.transform.parent.gameObject.SetActive(true);
                searchContentLoading.SetActive(false);
                return;
            }

            var findUser = await Services.SocialService.FindUserAsync(new FindUserRequest
            {
                keyword = searchTerm.Replace("#", "%23"), // the "#" character does not play well with web requests / urls
                userConstraints = searchFilterHashSet.ToArray(),
            });

            if (searchTerm == currentSearchTerm)
            {
                PopulateSearchPanel(findUser.Result);
                searchContentLoading.SetActive(false);
            }
        }

        private static async void RefreshOwnUserProfileAsync()
        {
            var userProfileResult = await Services.UserProfileService.GetUserProfileAsync(new GetUserProfileRequest());
            var userProfile = userProfileResult.Result.UserProfile; 
            UserProfileLocalDataManager.Instance.UpdateLocalUserProfile(userProfile);
        }

        private void UpdateSearchUI()
        {
            searchInputField.gameObject.SetActive(isSearchActive);
            searchToggleContainer.gameObject.SetActive(isSearchActive);
            searchContentPanel.gameObject.SetActive(isSearchActive);
            normalContentPanel.gameObject.SetActive(!isSearchActive);
            
            // update spacing for search area
            headerLayoutElement.minHeight =
                isSearchActive ? originalHeaderMinHeight + safeAreaHeightDelta : originalHeaderMinHeight;
            searchContainer.sizeDelta = new Vector2(searchContainer.sizeDelta.x,
                isSearchActive ? originalSearchContainerHeight + safeAreaHeightDelta : originalHeaderMinHeight);
        }
    }
}
