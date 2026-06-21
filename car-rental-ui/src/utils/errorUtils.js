const STATUS_LABELS = {
  400: 'Bad Request',
  401: 'Unauthorized',
  403: 'Forbidden',
  404: 'Not Found',
  408: 'Request Timeout',
  409: 'Conflict',
  422: 'Unprocessable Entity',
  429: 'Too Many Requests',
  500: 'Internal Server Error',
  502: 'Bad Gateway',
  503: 'Service Unavailable',
  504: 'Gateway Timeout',
}

/**
 * Returns a concise, user-facing error string based on HTTP status code.
 * Format: "400 Bad Request" or "Network Error — unable to reach the server."
 */
export function formatApiError(err) {
  const status = err?.response?.status

  if (!status) {
    // No response at all — network failure or CORS block
    return 'Network Error — unable to reach the server.'
  }

  const label = STATUS_LABELS[status] ?? 'Unexpected Error'
  return `${status} ${label}`
}
