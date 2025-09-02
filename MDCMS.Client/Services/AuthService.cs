using Blazored.LocalStorage;
using MDCMS.Client.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace MDCMS.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly ApiAuthenticationStateProvider _authProvider;

        public AuthService(HttpClient http, ILocalStorageService localStorage, AuthenticationStateProvider authProvider)
        {
            _http = http;
            _localStorage = localStorage;
            _authProvider = (ApiAuthenticationStateProvider)authProvider;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/login", new { username, password });

            if (!response.IsSuccessStatusCode) return false;

            var token = await response.Content.ReadAsStringAsync();

            await _localStorage.SetItemAsStringAsync("authToken", token);
            _authProvider.MarkUserAsAuthenticated(token);
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return true;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _authProvider.MarkUserAsLoggedOut();
            _http.DefaultRequestHeaders.Authorization = null;
        }
    }
}
