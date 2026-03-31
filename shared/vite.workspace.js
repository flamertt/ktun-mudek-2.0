import path from 'node:path'
import { fileURLToPath } from 'node:url'

const __dirname = path.dirname(fileURLToPath(import.meta.url))

/** Depo kökü (`ktun-mudek-2.0`) */
export const workspaceRoot = path.resolve(__dirname, '..')

/** `@shared/...` → `shared/...` */
export const workspaceResolve = {
  alias: {
    '@shared': path.join(workspaceRoot, 'shared'),
  },
}
