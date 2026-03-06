using ChimpType.Data;
using ChimpType.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace ChimpType.Provider
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _js;
        private readonly AuthService _auth;
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public CustomAuthStateProvider(AuthService auth, IJSRuntime js)
        {
            _js = js;
            _auth = auth;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var hasCircuit = await _js.InvokeAsync<bool>("eval", "typeof window !== 'undefined'");
                if (!hasCircuit) return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

                var token = await _js.InvokeAsync<string>("localStorage.getItem", "sessionToken");
                if (!string.IsNullOrEmpty(token))
                {
                    var user = await _auth.ValidateSessionToken(token);
                    if (user != null)
                    {
                        var claims = new List<Claim>
                        {
                            new(ClaimTypes.Name, user.Name),
                            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new("username", user.Username),
                            new(ClaimTypes.Email, user.Email),
                            new("sessionToken", token)
                        };

                        var identity = new ClaimsIdentity(claims, "custom");
                        return new AuthenticationState(new ClaimsPrincipal(identity));
                    } 
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error validating session token: {ex.Message}");
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public async Task Login(User user)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", "sessionToken", user.SessionTokenId);
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "sessionToken");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task Logout(string username)
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "sessionToken");
            if (!string.IsNullOrEmpty(token))
            {
                if (!string.IsNullOrEmpty(username))
                    await _auth.Logout(username);
                
                await _js.InvokeVoidAsync("localStorage.removeItem", "sessionToken");
            }

            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void Dispose() { }
    }
}
