using DAL.DTO;
using DAL.Repositories.ApplicationRepository;
using Firebase.Auth;
using Firebase.Storage;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Services.ApplicationService
{
    public class ApplicationService : IApplicationService
    {
        private static string _apiKey = "AIzaSyAWfeHe6r4CXfYsvmV1RYnBvv3bkVCY8NA";
        private static string _bucket = "skillslab2023assignment-e85e2.appspot.com";
        private static string _authEmail = "hishaam.munsoor@ceridian.com";
        private static string _authPassword = "Hishaam22011403!";

        private readonly IApplicationRepository _applicationRepository;
        public ApplicationService(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<string> Upload(Stream stream, string fileName)
        {
            try
            {
                // Authenticate with firebase using email and password
                var auth = new FirebaseAuthProvider(new FirebaseConfig(_apiKey));
                var authResult = await auth.SignInWithEmailAndPasswordAsync(_authEmail, _authPassword);

                var cancellation = new CancellationTokenSource();

                // Set up firebase storage with authentication token and options
                var storage = new FirebaseStorage(_bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(authResult.FirebaseToken),
                        ThrowOnCancel = true // Throw exception if upload is cancelled
                    });

                var uploadTask = storage
                    .Child("images")
                    .Child(fileName)
                    .PutAsync(stream, cancellation.Token);

                // Send download link in repository to store url in table
                string downloadLink = await uploadTask;
                return downloadLink;
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
