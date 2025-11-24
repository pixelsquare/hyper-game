using Cysharp.Threading.Tasks;

namespace Santelmo.Rinsurv.Backend
{
    public interface IAuthService : IService
    {
        public string UserId { get; }
        public bool IsAlreadySigned { get; }
        
        public UniTask<ServiceResultWrapper<SignInGoogleResult>> SignInGoogleAsync(SignInGoogleRequest request);
        public UniTask<ServiceResultWrapper<SignInFacebookResult>> SignInFacebookAsync(SignInFacebookRequest request);
        public UniTask<ServiceResultWrapper<SignInEmailResult>> SignInEmailAsync(SignInEmailRequest request);
        public UniTask<ServiceResultWrapper<SignInFirebaseResult>> SignInFirebaseAsync(SignInFirebaseRequest request);
        public UniTask<ServiceResultWrapper<SignInAnonymouslyResult>> SignInAnonymouslyAsync(SignInAnonymouslyRequest request);
        public UniTask<ServiceResultWrapper<SignOutResult>> SignOutAsync(SignOutRequest request);
        public UniTask<ServiceResultWrapper<CreateUserWithEmailResult>> CreateUserWithEmailAsync(CreateUserWithEmailRequest request);
        public UniTask<ServiceResultWrapper<SendEmailVerificationResult>> SendEmailVerificationAsync();
        public UniTask<ServiceResultWrapper<SendPasswordResetResult>> SendPasswordResetAsync(SendPasswordResetRequest request);
    }
}
