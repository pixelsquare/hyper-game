using System;
using System.Collections.Generic;
using System.Globalization;
using Moq;
using NUnit.Framework;
using Santelmo.Rinsurv.Backend;
using Task = System.Threading.Tasks.Task;

namespace Santelmo.Rinsurv.Editor.Tests
{
    public class FirestoreCloudSyncEditorTests
    {
        private ICloudSyncService service;
        private Dictionary<string, object> mockData;
        
        [OneTimeSetUp]
        public void Initialize()
        {
            mockData = new Dictionary<string, object>
            {
                {"one", DateTime.Now.ToString(CultureInfo.InvariantCulture)},
                {"two", DateTime.Now.Day},
                {"three", DateTime.Now.Hour},
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(x => x.UserId).Returns("player_id_test");
            service = FirestoreCloudSyncService.Create(mockAuthService.Object);
        }

        [Test]
        public async Task WriteAndReadFromDatabaseAsync()
        {
            await service.WriteAsync("test_key", mockData);
            var retrievedDictionary = await service.ReadAsync<Dictionary<string, object>>("test_key");
            Assert.IsNotNull(retrievedDictionary);
            foreach (var d in mockData)
            {
                Assert.That(retrievedDictionary.ContainsKey(d.Key), Is.True);
                Assert.That(mockData[d.Key], Is.EqualTo(retrievedDictionary[d.Key]));
            }
        }
    }
}
