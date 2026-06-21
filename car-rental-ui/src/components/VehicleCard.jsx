const PROVIDER_COLORS = {
  PremiumDrive: { bg: '#ebf8ff', color: '#2b6cb0', border: '#bee3f8' },
  BudgetWheels: { bg: '#f0fff4', color: '#276749', border: '#9ae6b4' },
}

const CATEGORY_ICONS = {
  Economy: '🚗',
  Compact: '🚙',
  SUV: '🛻',
  Minivan: '🚐',
}

export default function VehicleCard({ vehicle, onBook }) {
  const providerStyle = PROVIDER_COLORS[vehicle.provider] || {
    bg: '#f7fafc', color: '#4a5568', border: '#e2e8f0',
  }

  return (
    <div className="card" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '16px', padding: '20px 24px' }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: '16px', flex: 1, minWidth: '200px' }}>
        <span style={{ fontSize: '2rem' }}>{CATEGORY_ICONS[vehicle.category] || '🚗'}</span>
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '4px' }}>
            <span
              style={{
                background: providerStyle.bg,
                color: providerStyle.color,
                border: `1px solid ${providerStyle.border}`,
                borderRadius: '12px',
                padding: '2px 10px',
                fontSize: '0.78rem',
                fontWeight: 700,
                letterSpacing: '0.02em',
              }}
            >
              {vehicle.provider}
            </span>
            <span style={{ fontWeight: 700, fontSize: '1rem' }}>{vehicle.category}</span>
          </div>
          <div style={{ fontSize: '0.85rem', color: '#718096', marginBottom: '2px' }}>
            {vehicle.cancellationPolicy}
          </div>
          <div style={{ fontSize: '0.85rem' }}>
            {vehicle.includesInsurance
              ? <span style={{ color: '#276749' }}>✓ Comprehensive Insurance</span>
              : <span style={{ color: '#c05621' }}>⚠ Basic Insurance Only</span>}
          </div>
        </div>
      </div>

      <div style={{ textAlign: 'right', minWidth: '140px' }}>
        <div style={{ fontSize: '0.8rem', color: '#718096' }}>
          £{vehicle.perDayRate.toFixed(2)}/day
        </div>
        <div style={{ fontSize: '1.5rem', fontWeight: 700, color: '#1a202c' }}>
          £{vehicle.totalPrice.toFixed(2)}
        </div>
        <div style={{ fontSize: '0.78rem', color: '#718096', marginBottom: '10px' }}>total</div>
        <button className="btn-primary" onClick={onBook} style={{ width: '100%' }}>
          Book
        </button>
      </div>
    </div>
  )
}
