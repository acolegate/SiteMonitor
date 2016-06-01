using System.Net.Http;
using System.Timers;

using HttpAgentManager;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace HttpAgentManagerTests
{
    [TestClass]
    public class AgentTests
    {
        private const int TestAgentId = 1;
        private Agent _classUnderTest;
        private Mock<HttpClient> _mockHttpClient;
        private Mock<Timer> _mockTimer;

        [TestInitialize]
        public void Initialise()
        {
            _mockHttpClient = new Mock<HttpClient>(MockBehavior.Strict);
            _mockTimer = new Mock<Timer>(MockBehavior.Strict);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _mockHttpClient.VerifyAll();
            _mockTimer.VerifyAll();
        }

        [TestMethod]
        public void Agent_Constructor()
        {
            // arrange
            Config.Site testSite = new Config.Site
                                       {
                                       };

            _classUnderTest = new Agent(TestAgentId, testSite, _mockHttpClient.Object, _mockTimer.Object);

            // act

            // assert
            Assert.IsInstanceOfType(_classUnderTest, typeof(Agent), "Unexpected instance of type type created");
        }
    }
}