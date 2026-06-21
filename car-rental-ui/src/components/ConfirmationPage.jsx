const CATEGORY_ICONS = { Economy: '🚗', Compact: '🚙', SUV: '🛻', Minivan: '🚐' }

export default function ConfirmationPage({ confirmation, onSearchAgain }) {
  return (
    <div>
      <div className="card" style={{ textAlign: 'center', background: 'linear-gradient(135deg, #f0fff4, #ebf8ff)', border: '1px solid #9ae6b4', marginBottom: '16px' }}>
        <div style={{ fontSize: '3rem', marginBottom: '8px' }}>✅</div>
        <h2 style={{ color: '#276749', marginBottom: '8px' }}>Booking Confirmed!</h2>
        <div style={{ fontSize: '0.85rem', color: '#718096', marginBottom: '8px' }}>Reference Number</div>
        <div
          style={{
            display: 'inline-block',
            fontSize: '1.8rem',
            fontWeight: 800,
            color: '#2b6cb0',
            background: 'white',
            border: '2px solid #bee3f8',
            borderRadius: '10px',
            padding: '10px 24px',
            letterSpacing: '0.06em',
            marginBottom: '8px',
          }}
        >
          {confirmation.reference}
        </div>
      </div>

      <div className="card">
        <h2 style={{ marginBottom: '20px' }}>Booking Summary</h2>
        <div style={{ display: 'grid', gap: '14px' }}>
          <SummaryRow label="Provider" value={confirmation.provider} />
          <SummaryRow
            label="Vehicle"
            value={`${CATEGORY_ICONS[confirmation.category] || '🚗'} ${confirmation.category}`}
          />
          <SummaryRow label="Pickup Location" value={confirmation.pickupLocation} />
          <SummaryRow label="Pickup Date" value={confirmation.from} />
          <SummaryRow label="Return Date" value={confirmation.to} />
          <SummaryRow
            label="Total Price"
            value={`£${typeof confirmation.totalPrice === 'number' ? confirmation.totalPrice.toFixed(2) : confirmation.totalPrice}`}
            bold
          />
          <SummaryRow label="Cancellation Policy" value={confirmation.cancellationPolicy} />
        </div>

        <button
          className="btn-primary"
          onClick={onSearchAgain}
          style={{ marginTop: '24px', width: '100%' }}
        >
          🔍 Search Again
        </button>
      </div>
    </div>
  )
}

function SummaryRow({ label, value, bold }) {
  return (
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', gap: '16px', paddingBottom: '12px', borderBottom: '1px solid #e2e8f0' }}>
      <span style={{ fontSize: '0.875rem', color: '#718096', minWidth: '140px' }}>{label}</span>
      <span style={{ fontWeight: bold ? 700 : 500, textAlign: 'right' }}>{value}</span>
    </div>
  )
}
