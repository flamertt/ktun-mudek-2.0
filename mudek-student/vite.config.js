import path from 'node:path'
import { fileURLToPath } from 'node:url'
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

const __dirname = path.dirname(fileURLToPath(import.meta.url))
const nm = path.join(__dirname, 'node_modules')

/** Backend kök URL (deploy’da güncelleyin) */
const API_BASE_URL = 'http://localhost:5010'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  base: '/student/',
  define: {
    'import.meta.env.VITE_API_BASE_URL': JSON.stringify(API_BASE_URL),
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
