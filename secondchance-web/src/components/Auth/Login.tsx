import React, { useState } from 'react';
import { authApi } from '../../api/Auth/authApi';

interface LoginProps {
  onSwitchToRegister: () => void;
}

export default function Login({ onSwitchToRegister }: LoginProps) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [copiedKey, setCopiedKey] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const data = await authApi.login(email, password);
      sessionStorage.setItem('token', data.token);

      alert('Login success');
      window.location.href = '/';
    } catch (err: any) {
      setError(
        err.response?.data?.message || 'Login failed, please check your email or password'
      );
    } finally {
      setLoading(false);
    }
  };

  // Helper to fill the inputs and copy to clipboard
  const handleFillCredentials = (targetEmail: string, targetPass: string, key: string) => {
    setEmail(targetEmail);
    setPassword(targetPass);
    navigator.clipboard.writeText(targetEmail);
    setCopiedKey(key);
    setTimeout(() => setCopiedKey(null), 2000);
  };

  return (
    <div
      style={{
        maxWidth: '420px',
        margin: '40px auto',
        padding: '24px',
        border: '1px solid #e2e8f0',
        borderRadius: '12px',
        boxShadow: '0 4px 12px rgba(0, 0, 0, 0.05)',
        backgroundColor: '#ffffff',
        fontFamily: 'system-ui, -apple-system, sans-serif',
      }}
    >
      <h2 style={{ marginTop: 0, color: '#1e293b', fontSize: '1.4rem' }}>
        🔑 SecondChance Login
      </h2>

      {error && (
        <div
          style={{
            padding: '10px 12px',
            backgroundColor: '#fef2f2',
            border: '1px solid #fecaca',
            color: '#dc2626',
            borderRadius: '6px',
            fontSize: '0.875rem',
            marginBottom: '15px',
          }}
        >
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '6px', fontWeight: 500, color: '#475569', fontSize: '0.9rem' }}>
            Email:
          </label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            placeholder="Enter your email"
            style={{
              width: '100%',
              padding: '10px',
              borderRadius: '6px',
              border: '1px solid #cbd5e1',
              boxSizing: 'border-box',
              fontSize: '0.95rem',
            }}
          />
        </div>

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '6px', fontWeight: 500, color: '#475569', fontSize: '0.9rem' }}>
            Password:
          </label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            placeholder="Enter your password"
            style={{
              width: '100%',
              padding: '10px',
              borderRadius: '6px',
              border: '1px solid #cbd5e1',
              boxSizing: 'border-box',
              fontSize: '0.95rem',
            }}
          />
        </div>

        <button
          type="submit"
          disabled={loading}
          style={{
            width: '100%',
            padding: '12px',
            backgroundColor: loading ? '#93c5fd' : '#2563eb',
            color: 'white',
            border: 'none',
            borderRadius: '6px',
            cursor: loading ? 'not-allowed' : 'pointer',
            fontWeight: 600,
            fontSize: '1rem',
            transition: 'background-color 0.2s',
          }}
        >
          {loading ? 'Logging you in...' : 'Login'}
        </button>

        <p style={{ marginTop: '16px', textAlign: 'center', color: '#64748b', fontSize: '0.9rem' }}>
          Don't have an account?{' '}
          <button
            type="button"
            onClick={onSwitchToRegister}
            style={{
              background: 'none',
              border: 'none',
              color: '#2563eb',
              cursor: 'pointer',
              textDecoration: 'underline',
              fontWeight: 500,
            }}
          >
            Register here
          </button>
        </p>
      </form>

      {/* Styled Test Logins Section */}
      <div
        style={{
          marginTop: '24px',
          paddingTop: '20px',
          borderTop: '1px solid #f1f5f9',
        }}
      >
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '12px' }}>
          <h3 style={{ margin: 0, fontSize: '0.95rem', color: '#334155', fontWeight: 600 }}>
            ⚡ Demo Credentials
          </h3>
          <span style={{ fontSize: '0.75rem', color: '#64748b', backgroundColor: '#f1f5f9', padding: '2px 8px', borderRadius: '12px' }}>
            Click to Auto-fill
          </span>
        </div>

        <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
          {/* User Card */}
          <div
            style={{
              backgroundColor: '#f8fafc',
              border: '1px solid #e2e8f0',
              borderRadius: '8px',
              padding: '10px 12px',
              fontSize: '0.85rem',
            }}
          >
            <div style={{ fontWeight: 600, color: '#059669', marginBottom: '4px', display: 'flex', alignItems: 'center', gap: '6px' }}>
              <span style={{ width: '8px', height: '8px', borderRadius: '50%', backgroundColor: '#10b981', display: 'inline-block' }}></span>
              Standard User
            </div>
            <div style={{ color: '#475569' }}>Email: <code style={{ backgroundColor: '#e2e8f0', padding: '1px 4px', borderRadius: '4px' }}>user@example.com</code></div>
            <div style={{ color: '#475569', marginTop: '2px' }}>Pass: <code style={{ backgroundColor: '#e2e8f0', padding: '1px 4px', borderRadius: '4px' }}>P@ssw0rd!1234567890</code></div>
            <button
              type="button"
              onClick={() => handleFillCredentials('user@example.com', 'P@ssw0rd!1234567890', 'user')}
              style={{
                marginTop: '8px',
                width: '100%',
                padding: '4px 8px',
                backgroundColor: '#ffffff',
                border: '1px solid #cbd5e1',
                borderRadius: '4px',
                color: '#334155',
                fontSize: '0.75rem',
                fontWeight: 500,
                cursor: 'pointer',
              }}
            >
              {copiedKey === 'user' ? '✓ Auto-filled & Copied!' : 'Use User Credentials'}
            </button>
          </div>

          {/* Admin Card */}
          <div
            style={{
              backgroundColor: '#f8fafc',
              border: '1px solid #e2e8f0',
              borderRadius: '8px',
              padding: '10px 12px',
              fontSize: '0.85rem',
            }}
          >
            <div style={{ fontWeight: 600, color: '#d97706', marginBottom: '4px', display: 'flex', alignItems: 'center', gap: '6px' }}>
              <span style={{ width: '8px', height: '8px', borderRadius: '50%', backgroundColor: '#f59e0b', display: 'inline-block' }}></span>
              Administrator
            </div>
            <div style={{ color: '#475569' }}>Email: <code style={{ backgroundColor: '#e2e8f0', padding: '1px 4px', borderRadius: '4px' }}>admin@secondchance.com</code></div>
            <div style={{ color: '#475569', marginTop: '2px' }}>Pass: <code style={{ backgroundColor: '#e2e8f0', padding: '1px 4px', borderRadius: '4px' }}>SecurePassword123!</code></div>
            <button
              type="button"
              onClick={() => handleFillCredentials('admin@secondchance.com', 'SecurePassword123!', 'admin')}
              style={{
                marginTop: '8px',
                width: '100%',
                padding: '4px 8px',
                backgroundColor: '#ffffff',
                border: '1px solid #cbd5e1',
                borderRadius: '4px',
                color: '#334155',
                fontSize: '0.75rem',
                fontWeight: 500,
                cursor: 'pointer',
              }}
            >
              {copiedKey === 'admin' ? '✓ Auto-filled & Copied!' : 'Use Admin Credentials'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}