import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router'
// @ts-ignore
import vue3GoogleLogin from 'vue3-google-login'

const app = createApp(App)

app.use(router)
app.use(vue3GoogleLogin, {
    clientId: import.meta.env.VITE_GOOGLE_CLIENTID
})

app.mount('#app')
