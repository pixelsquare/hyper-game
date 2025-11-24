using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Santelmo.Rinsurv.Backend;

namespace Santelmo.Rinsurv.Editor.Tests
{
    public class CloudSyncEditorTests
    {
        private Dictionary<string, object> mockDatabase;

        [OneTimeSetUp]
        public void Initialize()
        {
            mockDatabase = new Dictionary<string, object>
            {
                {"one", 1},
            };
        }

        private static IEnumerable ReadDatabaseInputOutput
        {
            get
            {
                yield return new TestCaseData("one", 1).SetName("In database");
                yield return new TestCaseData("missing", null).SetName("Not in database");
            }
        }

        [TestCaseSource(nameof(ReadDatabaseInputOutput))]
        public async Task ReadFromDatabaseAsync(string input, object expectedReadData)
        {
            var mockSyncModule = new Mock<ICloudSyncService>();
            string key = null;
            mockSyncModule.Setup(x => x.ReadAsync<object>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((r, _) => key = r)
                .Returns(() =>
                {
                    if (mockDatabase.TryGetValue(key, out var result))
                    {
                        return new UniTask<object>(result);
                    }

                    return new UniTask<object>(null);
                });
            var result = await mockSyncModule.Object.ReadAsync<object>(input);
            Assert.AreEqual(result, expectedReadData);
        }

        [Test]
        public async Task WriteToDatabaseAsync()
        {
            var mockSyncModule = new Mock<ICloudSyncService>();
            mockSyncModule.Setup(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Callback<string, object, CancellationToken>((keyArg, objArg, _) => mockDatabase[keyArg] = objArg)
                .Returns(() => new UniTask<bool>(true));
            var result = await mockSyncModule.Object.WriteAsync("two", 2);
            Assert.IsTrue(result);
            Assert.AreEqual(2, mockDatabase["two"]);
        }
    }
}
