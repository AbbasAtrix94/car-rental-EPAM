import { useState } from 'react'

const DOMESTIC_CITIES = ['london', 'manchester']

// Normalise property name — handles both camelCase (API default) and PascalCase
function getPickupLocation(vehicle) {
  return vehicle.pickupLocation || vehicle.PickupLocation || ''
}

function isInternational(location) {
  if (!location) return false
  return !DOMESTIC_CITIES.includes(location.toLowerCase().trim())
}

const CATEGORY_ICONS = { Economy: '🚗', Compact: '🚙', SUV: '🛻', Minivan: '🚐' }

export default function BookingForm({ vehicle, onConfirm, onBack, loading, error }) {
  const [driverName, setDriverName] = useState('')
  const [documentType, setDocumentType] = useState('Passport')
  const [documentNumber, setDocumentNumber] = useState('')
  const [validationError, setValidationError] = useState('')

  const international = isInternational(getPickupLocation(vehicle))

  function handleSubmit(e) {
    e.preventDefault()
    setValidationError('')

    if (!driverName.trim()) {
      setValidationError('Driver name is required.')
      return
    }
    if (!documentNumber.trim()) {
      setValidationError('Document number is required.')
      return
    }
    if (international && documentType === 'NationalId') {
      setValidationError('International pickups require a Passport.')
      return
    }

    onConfirm({
      vehicleResultId: vehicle.id,
      driverName: driverName.trim(),
      documentType,
      documentNumber: documentNumber.trim(),
    })
  }

  return (
    <div>
      <div className="card" style={{ background: '#f7fafc', borderLeft: '4px solid #3182ce', padding: '16px 20px', marginBottom: '16px' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
          <span style={{ fontSize: '1.5rem' }}>{CATEGORY_ICONS[vehicle.category] || '🚗'}</span>
          <div>
            <div style={{ fontWeight: 700 }}>{vehicle.provider} — {vehicle.category}</div>
            <div style={{ fontSize: '0.85rem', color: '#718096' }}>
              {vehicle.pickupLocation} · {vehicle.from} → {vehicle.to}
            </div>
            <div style={{ fontWeight: 700, color: '#2d3748', marginTop: '4px' }}>
              £{vehicle.totalPrice?.toFixed(2)} total
            </div>
          </div>
        </div>
      </div>

      <div className="card">
        <h2>Driver Details</h2>

        {(validationError || error) && (
          <div className="error-msg">{validationError || error}</div>
        )}

        <form onSubmit={handleSubmit}>
          <div style={{ display: 'grid', gap: '16px' }}>
            <div>
              <label htmlFor="driverName">Full Name</label>
              <input
                id="driverName"
                type="text"
                placeholder="As it appears on your document"
                value={driverName}
                onChange={e => setDriverName(e.target.value)}
              />
            </div>

            <div>
              <label htmlFor="docType">Document Type</label>
              <select
                id="docType"
                value={documentType}
                onChange={e => setDocumentType(e.target.value)}
              >
                <option value="Passport">Passport</option>
                <option value="NationalId" disabled={international}>
                  National ID{international ? ' (not accepted for international pickup)' : ''}
                </option>
              </select>
              {international && (
                <p style={{ fontSize: '0.8rem', color: '#718096', marginTop: '4px' }}>
                  ℹ International pickup — Passport required
                </p>
              )}
            </div>

            <div>
              <label htmlFor="docNumber">Document Number</label>
              <input
                id="docNumber"
                type="text"
                placeholder="e.g. P123456789"
                value={documentNumber}
                onChange={e => setDocumentNumber(e.target.value)}
              />
            </div>

            <div style={{ display: 'flex', gap: '12px', marginTop: '8px' }}>
              <button type="button" className="btn-secondary" onClick={onBack} style={{ flex: 1 }}>
                ← Back
              </button>
              <button type="submit" className="btn-success" disabled={loading} style={{ flex: 2 }}>
                {loading ? 'Confirming…' : 'Confirm Booking'}
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  )
}
