import path from 'node:path'
import { fileURLToPath } from 'node:url'
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

const __dirname = path.dirname(fileURLToPath(import.meta.url))
const nm = path.join(__dirname, 'node_modules')

/** BitirmeApi (http profili). HTTPS-only çalıştırıyorsanız https://localhost:7097 */
const DEV_API_TARGET = process.env.VITE_DEV_API_PROXY_TARGET ?? 'http://localhost:5010'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: DEV_API_TARGET,
        changeOrigin: true,
        secure: false,
        configure(proxy) {
          proxy.on('proxyReq', (proxyReq, req) => {
            const auth = req.headers.authorization
            if (auth) proxyReq.setHeader('Authorization', auth)
          })
        },
      },
    },
  },
  resolve: {
    alias: {
      // Uygulama ayrı deploy edilecekse `root/shared` yerine uygulama içindeki shared'i kullan.
      '@shared': path.join(__dirname, 'src/shared'),
      react: path.join(nm, 'react'),
      'react-dom': path.join(nm, 'react-dom'),
      'lucide-react': path.join(nm, 'lucide-react'),
    },
  },
})
