import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router'
// @ts-ignore
import vue3GoogleLogin from 'vue3-google-login'

const app = createApp(App)

app.use(router)
app.use(vue3GoogleLogin, {
    // TODO: Replace with your actual Google Client ID from Google Cloud Console
    clientId: '1041931643387-ad6fmi62cue4v1ovodsl3m34j7q2bj3l.apps.googleusercontent.com'
})

app.mount('#app')
