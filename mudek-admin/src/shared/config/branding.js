/**
 * Ortak görsel kimlik (mudek-admin / mudek-student / mudek-teacher).
 * Tablo avatarları ve ileride marka görselleri için tek kaynak.
 *
 * URL’leri burada güncelleyin; harici host süresi dolabilir.
 * Bileşenler `import { branding, pickUserAvatarUrl } from '@shared/config/branding.js'` kullanır.
 */

/** @type {readonly string[]} */
const USER_AVATAR_POOL = [
  'https://lh3.googleusercontent.com/aida-public/AB6AXuDo2mu2CQRMR0VeJBCPNv42qWNeo9lYHo2-wgSbtteKTHazeh4551Q8qznG7Y6PYjg4YHbQ_-x0Sgu8v7mAU2uaZOVdgkSURicFa6-hyvAox_uEDjBAipTrpvH-E2dfnS23ovxEVbfeNdFT6JAsnKW_QR1UfOwRYYuLV2v6_dISCgZKA_xD_dmAMryDoNsImJky3EHALZN--HC6HDQGd0WzyuQlomo-bZXjgNuXIl-wehqMWaHznp4DqeuWI4_qoC1H5hZOt6ASyCI',
  'https://lh3.googleusercontent.com/aida-public/AB6AXuBVl_-58JYipQSdVuiVF_qeBX_BoZw6k_h_kfX0ZgOcKNb7lZ9-BOwVW5xLBrtMsIZl-Dfw7ybvqvqtcG_xu4Lrqa6tBivobsQIr9oPVc0fECRtTq63K9iThym6QpIulLFpp9RBQPXl9Nq-koL-UnMEHWL5B9bcLUMxb_DUk14qneV8PUxNOYvXC-chDmNrB1nz-9p-KSWK9gQPU9lJWWTR15UilbmaUxwaMii8Kfi7myVyMGCWgVf6JPYx5BmKC48LT6JuJeRe5EM',
  'https://lh3.googleusercontent.com/aida-public/AB6AXuCYzhY-juxcXf5p77oWVjGRnxZD-wi8hpi4MTGiDrcIom6U0oeiLNk0sZV4FTf5aLPaGVIBu6zXHC7cvwEAQkDMklG_ZvtNsRe6LtxXzSbEEne1f-wHKzwfacV2I5ahkRF-hk40M7DPOGGFQ9q6kfybs0MIggfm3AmN5I3UtaCzpb4-WqvFv0qoLOGVAThUf90E2drbrwIy8IX_NnqLTaJknrtinfNREJSAg3x4SCkZQQ0HsDC_n8eMojL_PraU_JTb6I0nrPiK2RI',
  'https://lh3.googleusercontent.com/aida-public/AB6AXuADolp9YlpOvkinDH2mLTK6-_ErP8YFJmBjhEDDlCZQVczMlMgTfJIV-4YrHfJhLt0eNo2-uQWSBTvuIOl9nu9KqXUNZsIro2MbDw_Gfg0omU-gIYtuB9jnFjrW5BRKcYWM7O7PB_I-LaIaOFtca44SEvOzkfyRZWv8ZRrUQ69XepvbelylP-OOxmK48PVG1ztcdBiBs0bwpY9wauGSeivMwLnFUDGC5YkFQy9rCBcTgJ0Ch5hdz7Leqpn5RjJyyPXGOhNh_0_P9QQ',
]

function hashString(s) {
  let h = 0
  for (let i = 0; i < s.length; i += 1) {
    h = (Math.imul(31, h) + s.charCodeAt(i)) | 0
  }
  return Math.abs(h)
}

/**
 * Aynı seed için her zaman aynı görsel; farklı satırlar havuzda çeşitlenir.
 * @param {string} seed
 * @param {'teacher' | 'student'} variant
 * @returns {string}
 */
export function pickUserAvatarUrl(seed, variant) {
  const key = `${variant}\0${seed}`
  const idx = USER_AVATAR_POOL.length ? hashString(key) % USER_AVATAR_POOL.length : 0
  return USER_AVATAR_POOL[idx] ?? ''
}

export const branding = {
  avatars: {
    /** Tablo satırı avatar havuzu (öğretmen/öğrenci aynı havuzdan deterministik seçilir) */
    userPool: USER_AVATAR_POOL,
  },
}

