import { useState } from 'react'
import VehicleCard from './VehicleCard.jsx'

export default function ResultsList({ results, searchParams, onBook, onBack }) {
  const [sorted, setSorted] = useState(false)

  const displayed = sorted
    ? [...results].sort((a, b) => a.totalPrice - b.totalPrice)
    : results

  return (
    <div>
      <div className="card" style={{ marginBottom: '8px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '8px' }}>
          <div>
            <h2 style={{ marginBottom: 0 }}>Available Vehicles</h2>
            <p style={{ color: '#718096', fontSize: '0.875rem', marginTop: '4px' }}>
              {searchParams.pickup} · {searchParams.from} → {searchParams.to}
              {searchParams.category ? ` · ${searchParams.category}` : ''}
            </p>
          </div>
          <div style={{ display: 'flex', gap: '8px' }}>
            <button
              className="btn-secondary"
              onClick={() => setSorted(s => !s)}
              style={{ fontSize: '0.85rem', padding: '8px 14px' }}
            >
              {sorted ? '✓ Sorted by Price' : 'Sort by Total Price'}
            </button>
            <button
              className="btn-secondary"
              onClick={onBack}
              style={{ fontSize: '0.85rem', padding: '8px 14px' }}
            >
              ← New Search
            </button>
          </div>
        </div>
      </div>

      {displayed.length === 0 ? (
        <div className="card" style={{ textAlign: 'center', color: '#718096', padding: '48px' }}>
          <div style={{ fontSize: '2rem', marginBottom: '12px' }}>🚫</div>
          <p style={{ fontSize: '1.1rem', fontWeight: 600 }}>No vehicles available</p>
          <p style={{ marginTop: '8px' }}>Try different dates or a different location.</p>
        </div>
      ) : (
        <div style={{ display: 'grid', gap: '12px' }}>
          {displayed.map(vehicle => (
            <VehicleCard key={vehicle.id} vehicle={vehicle} onBook={() => onBook(vehicle)} />
          ))}
        </div>
      )}
    </div>
  )
}
