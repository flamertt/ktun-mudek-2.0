import {
  flexRender,
  getCoreRowModel,
  getFilteredRowModel,
  getPaginationRowModel,
  useReactTable,
} from '@tanstack/react-table'
import { ChevronLeft, ChevronRight, Search } from 'lucide-react'

import styles from './DataTable.module.css'

/**
 * @template T
 * @param {{
 *   columns: import('@tanstack/react-table').ColumnDef<T, any>[],
 *   data: T[],
 *   globalFilter: string,
 *   onGlobalFilterChange: (v: string) => void,
 *   searchPlaceholder?: string,
 *   toolbarFilters?: import('react').ReactNode,
 *   toolbarExtra?: import('react').ReactNode,
 *   isLoading?: boolean,
 *   pageSize?: number,
 * }} props
 */
export function DataTable({
  columns,
  data,
  globalFilter,
  onGlobalFilterChange,
  searchPlaceholder = 'Tabloda ara…',
  toolbarFilters,
  toolbarExtra,
  isLoading,
  pageSize = 10,
}) {
  const table = useReactTable({
    data,
    columns,
    state: { globalFilter },
    onGlobalFilterChange: (updater) => {
      const next = typeof updater === 'function' ? updater(globalFilter ?? '') : updater
      onGlobalFilterChange(next ?? '')
    },
    globalFilterFn: 'includesString',
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    initialState: { pagination: { pageSize } },
  })

  return (
    <div className={styles.wrap}>
      <div className={styles.toolbar}>
        <div className={styles.toolbarMain}>
          {toolbarFilters}
          <label className={styles.searchLabel}>
            <span className={styles.visuallyHidden}>Ara</span>
            <Search className={styles.searchIcon} aria-hidden />
            <input
              className={styles.searchInput}
              type="search"
              value={globalFilter ?? ''}
              onChange={(e) => onGlobalFilterChange(e.target.value)}
              placeholder={searchPlaceholder}
              autoComplete="off"
            />
          </label>
        </div>
        {toolbarExtra ? <div className={styles.toolbarExtra}>{toolbarExtra}</div> : null}
      </div>

      {isLoading ? (
        <p className={styles.loading}>Yükleniyor…</p>
      ) : (
        <>
          <div className={styles.tableScroll}>
            <table className={styles.table}>
              <thead>
                {table.getHeaderGroups().map((headerGroup) => (
                  <tr key={headerGroup.id}>
                    {headerGroup.headers.map((header) => (
                      <th key={header.id} className={styles.th} scope="col">
                        {header.isPlaceholder
                          ? null
                          : flexRender(header.column.columnDef.header, header.getContext())}
                      </th>
                    ))}
                  </tr>
                ))}
              </thead>
              <tbody>
                {table.getRowModel().rows.map((row) => (
                  <tr key={row.id} className={styles.tr}>
                    {row.getVisibleCells().map((cell) => (
                      <td key={cell.id} className={styles.td}>
                        {flexRender(cell.column.columnDef.cell, cell.getContext())}
                      </td>
                    ))}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {table.getFilteredRowModel().rows.length === 0 ? (
            <p className={styles.empty}>Filtreye uygun satır yok.</p>
          ) : null}

          <div className={styles.pagination}>
            <button
              type="button"
              className={styles.pageBtn}
              disabled={!table.getCanPreviousPage()}
              onClick={() => table.previousPage()}
              aria-label="Önceki sayfa"
            >
              <ChevronLeft size={18} aria-hidden />
            </button>
            <span className={styles.pageInfo}>
              Sayfa {table.getState().pagination.pageIndex + 1} / {table.getPageCount() || 1}
            </span>
            <button
              type="button"
              className={styles.pageBtn}
              disabled={!table.getCanNextPage()}
              onClick={() => table.nextPage()}
              aria-label="Sonraki sayfa"
            >
              <ChevronRight size={18} aria-hidden />
            </button>
          </div>
        </>
      )}
    </div>
  )
}
