<script setup lang="ts">
import { useRouter } from 'vue-router';
// @ts-ignore
import { GoogleLogin } from 'vue3-google-login';

const router = useRouter();

const callback = (response: any) => {
  console.log("Logged in", response);
  if (response.credential) {
    localStorage.setItem('google_id_token', response.credential);
    
    // Decode and store user info
    try {
      const base64Url = response.credential.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function(c) {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
      
      const payload = JSON.parse(jsonPayload);
      if (payload.email) {
         const userSlug = payload.email.replace(/@/g, '-').replace(/\./g, '-');
         localStorage.setItem('user_slug', userSlug);
         console.log('Stored UserSlug:', userSlug);
      }
    } catch (e) {
      console.error('Error decoding token:', e);
    }

    router.push('/');
  }
};
</script>

<template>
  <div class="login-container">
    <div class="login-card">
      <h1>SmartEscape Planner</h1>
      <p>Please sign in to continue</p>
      
      <div class="google-btn-wrapper">
        <GoogleLogin :callback="callback" />
      </div>
    </div>
  </div>
</template>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.login-card {
  background: white;
  padding: 3rem;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.2);
  text-align: center;
  width: 100%;
  max-width: 400px;
}

h1 {
  margin-bottom: 0.5rem;
  color: #333;
}

p {
  color: #666;
  margin-bottom: 2rem;
}

.google-btn-wrapper {
  display: flex;
  justify-content: center;
}
</style>
