using Newtonsoft.Json;
using Xunit;

namespace Re.Core.Tests
{
    public sealed class VerificationResponseTests
    {
        [Fact]
        public void CanAssignScore()
        {
            var value = 0.5;
            var model = new VerificationResponse
            {
                Score = value
            };
            Assert.Equal(value, model.Score);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanAssignSuccess(bool value)
        {
            var model = new VerificationResponse
            {
                Success = value
            };
            Assert.Equal(value, model.Success);
        }

        [Fact]
        public void CanInstantiateConcreteInstance()
        {
            Assert.False(typeof(VerificationResponse).IsAbstract);
        }

        [Fact]
        public void DoesDeserializeFromJsonProperly()
        {
            var score = 0.7;
            var success = true;
            var json = $@"{{ ""score"": {score}, ""success"": {success.ToString().ToLower()} }}";
            var response = JsonConvert.DeserializeObject<VerificationResponse>(json);

            Assert.Equal(score, response.Score);
            Assert.Equal(success, response.Success);
        }
    }
}
