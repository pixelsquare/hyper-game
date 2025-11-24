using System.Threading.Tasks;
using Santelmo.Rinsurv.Backend;
using NUnit.Framework;
using UnityEngine;

namespace Santelmo.Rinsurv.Editor.Tests
{
    public class LoginEditorTests
    {
        private IAuthService _authService;
        private const string email = ""; // Email to be created/signed in.
        private const string password = ""; // Password used in creating/signed in.
        private const string emailReset = ""; // Email that you want to reset the password.
        
        [SetUp]
        public void Setup()
        {
            _authService = new FirebaseLoginService();
        }
        
                
        [TearDown]
        public async Task Login_SignOut()
        {
            await _authService.SignOutAsync(new SignOutRequest());
        }

        [Test]
        public async Task Login_CreateUser()
        {
            var emailSignInResult = await _authService.CreateUserWithEmailAsync(new CreateUserWithEmailRequest(email, password));
            Assert.That(emailSignInResult.Result, !Is.Null);
            
            Assert.That(emailSignInResult.Result.UserInfo, !Is.Null);
            Debug.Log($"Email: {emailSignInResult.Result.UserInfo.Email}");
            Debug.Log($"Display Name: {emailSignInResult.Result.UserInfo.DisplayName}");
            Debug.Log($"UserId: {emailSignInResult.Result.UserInfo.UserId}");
            Debug.Log($"IsVerified: {emailSignInResult.Result.UserInfo.IsEmailVerified}");
        }
        
        [Test]
        public async Task Login_Email()
        {
            var emailSignInResult = await _authService.SignInEmailAsync(new SignInEmailRequest(email, password));
            Assert.That(emailSignInResult.Result, !Is.Null);
            
            Assert.That(emailSignInResult.Result.UserInfo, !Is.Null);
            Debug.Log($"Email: {emailSignInResult.Result.UserInfo.Email}");
            Debug.Log($"Display Name: {emailSignInResult.Result.UserInfo.DisplayName}");
            Debug.Log($"UserId: {emailSignInResult.Result.UserInfo.UserId}");
            Debug.Log($"IsVerified: {emailSignInResult.Result.UserInfo.IsEmailVerified}");
        }    
        
        [Test]
        public async Task Login_SendPasswordReset()
        {
            var emailSignInResult = await _authService.SendPasswordResetAsync(new SendPasswordResetRequest(emailReset));
            Assert.That(emailSignInResult.Result, !Is.Null);
            
            Assert.That(emailSignInResult.Result.EmailSuccessful, !Is.False);
        }
    }
}
