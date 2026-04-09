import {
  BarChart3,
  BookOpen,
  Building2,
  CalendarRange,
  GraduationCap,
  Home,
  Layers,
  Link2,
  Percent,
  Settings,
  SquarePen,
  Target,
  Upload,
  UserRound,
  Users,
  UsersRound,
} from 'lucide-react'

/** @type {Record<string, import('react').ComponentType<{ className?: string; 'aria-hidden'?: string | boolean }>>} */
const NAV_ICON_BY_NAME = {
  home: Home,
  'book-open': BookOpen,
  'square-pen': SquarePen,
  target: Target,
  'bar-chart': BarChart3,
  users: Users,
  'graduation-cap': GraduationCap,
  building: Building2,
  'user-round': UserRound,
  'users-round': UsersRound,
  settings: Settings,
  layers: Layers,
  'link-2': Link2,
  percent: Percent,
  'calendar-range': CalendarRange,
  upload: Upload,
}

/**
 * Kenar çubuğu `appConfig.navSections` ikon anahtarı → Lucide bileşeni.
 * @param {string} [iconName]
 */
export function getNavIcon(iconName) {
  if (!iconName) return Home
  return NAV_ICON_BY_NAME[iconName] ?? Home
}
