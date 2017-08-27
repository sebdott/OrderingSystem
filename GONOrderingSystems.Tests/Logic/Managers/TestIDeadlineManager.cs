using GONOrderingSystems.Logic.Managers;
using GONOrderingSystems.Logic.Managers.Interface;
using System;
using Xunit;

namespace GONOrderingSystems.Tests.Logic.Managers
{
    public class TestIDeadlineManager
    {
        private IDeadlineManager _deadlineManager;
        
        public TestIDeadlineManager()
        {
            _deadlineManager = new DeadlineManager();   
        }

        [Fact]
        public async void TestCommitOrderSuccess()
        {
            try
            {
                var results = await _deadlineManager.GetDeadLineByEventId("TestEventId");

                Assert.NotNull(results);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }
    }
}
