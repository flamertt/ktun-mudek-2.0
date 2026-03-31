/**
 * Merkezi ortam sabitleri (üç Vite uygulaması ortak).
 * `VITE_API_BASE_URL` her uygulamanın `vite.config.js` içinde `define` ile verilir.
 */

const DEFAULT_API_BASE = 'http://localhost:5010'

/** Backend kök URL (şema + host + port, sonda / olmadan). */
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? DEFAULT_API_BASE
