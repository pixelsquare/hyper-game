using Kumu.Kulitan.Backend;
using Unity.VisualScripting;

public class UnitSetLocalUserProfile : Unit
{
    [DoNotSerialize] 
    [PortLabelHidden]
    public ControlInput enter;
    
    [DoNotSerialize] 
    public ControlOutput successExit;

    [DoNotSerialize] 
    public ControlOutput errorExit;
    
    [DoNotSerialize] public ValueInput userProfileResult;
    [DoNotSerialize] public ValueInput localDataManager;
    
    protected override void Definition()
    {
        enter = ControlInput(nameof(enter), flow =>
        {
            var dataManager = flow.GetValue<UserProfileLocalDataManager>(localDataManager);
            var result = flow.GetValue<GetUserProfileResult>(userProfileResult);
            var nickname = result.UserProfile.nickName;
            if (nickname == null || string.IsNullOrWhiteSpace(nickname))
            {
                return errorExit;
            }
            dataManager.UpdateLocalUserProfile(result.UserProfile);
            return successExit;
        });
        successExit = ControlOutput(nameof(successExit));
        errorExit = ControlOutput(nameof(errorExit)); 
        userProfileResult = ValueInput<GetUserProfileResult>(nameof(userProfileResult));
        localDataManager = ValueInput<UserProfileLocalDataManager>(nameof(localDataManager));
        
        Requirement(userProfileResult, enter);
        Requirement(localDataManager, enter);

        Succession(enter,successExit);
        Succession(enter,errorExit);
    }
}
