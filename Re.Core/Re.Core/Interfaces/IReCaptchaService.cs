using System.Threading.Tasks;

namespace Re.Core.Interfaces
{
    internal interface IReCaptchaService
    {
        Task<VerificationResponse> VerifyTokenAsync(string token);
    }
}
