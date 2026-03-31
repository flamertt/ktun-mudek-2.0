import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

import { workspaceResolve } from '../shared/vite.workspace.js'

/** Backend kök URL (deploy’da güncelleyin) */
const API_BASE_URL = 'http://localhost:5010'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  define: {
    'import.meta.env.VITE_API_BASE_URL': JSON.stringify(API_BASE_URL),
  },
  resolve: workspaceResolve,
})
