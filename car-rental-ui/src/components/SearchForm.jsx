import { useState } from 'react'

const CATEGORIES = ['Economy', 'Compact', 'SUV', 'Minivan']

export default function SearchForm({ onSearch, loading, error }) {
  const [pickup, setPickup] = useState('')
  const [from, setFrom] = useState('')
  const [to, setTo] = useState('')
  const [category, setCategory] = useState('')
  const [validationError, setValidationError] = useState('')

  function handleSubmit(e) {
    e.preventDefault()
    setValidationError('')

    if (!pickup.trim()) {
      setValidationError('Pickup location is required.')
      return
    }
    if (!from || !to) {
      setValidationError('Please select both pickup and return dates.')
      return
    }
    if (new Date(to) <= new Date(from)) {
      setValidationError('Return date must be after pickup date.')
      return
    }

    onSearch({ pickup: pickup.trim(), from, to, category: category || null })
  }

  return (
    <div className="card">
      <div className="header">
        <h1>🚗 SkyRoute Car Rental</h1>
        <p>Find the best vehicles across multiple providers</p>
      </div>

      {(validationError || error) && (
        <div className="error-msg">{validationError || error}</div>
      )}

      <form onSubmit={handleSubmit}>
        <div style={{ display: 'grid', gap: '16px' }}>
          <div>
            <label htmlFor="pickup">Pickup Location</label>
            <input
              id="pickup"
              type="text"
              placeholder="e.g. London, Paris, Tokyo"
              value={pickup}
              onChange={e => setPickup(e.target.value)}
            />
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
            <div>
              <label htmlFor="from">Pickup Date</label>
              <input
                id="from"
                type="date"
                value={from}
                min={new Date().toISOString().split('T')[0]}
                onChange={e => setFrom(e.target.value)}
              />
            </div>
            <div>
              <label htmlFor="to">Return Date</label>
              <input
                id="to"
                type="date"
                value={to}
                min={from || new Date().toISOString().split('T')[0]}
                onChange={e => setTo(e.target.value)}
              />
            </div>
          </div>

          <div>
            <label htmlFor="category">Vehicle Category (optional)</label>
            <select id="category" value={category} onChange={e => setCategory(e.target.value)}>
              <option value="">Any Category</option>
              {CATEGORIES.map(c => (
                <option key={c} value={c}>{c}</option>
              ))}
            </select>
          </div>

          <button type="submit" className="btn-primary" disabled={loading} style={{ marginTop: '8px' }}>
            {loading ? 'Searching…' : 'Search Vehicles'}
          </button>
        </div>
      </form>
    </div>
  )
}
