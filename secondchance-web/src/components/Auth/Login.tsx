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

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError('');
    setLoading(true);
    try {
      const data = await authApi.login(email, password);
      sessionStorage.setItem('token', data.token);
      window.location.href = '/';
    } catch (err: any) {
      setError(err.response?.status === 429
        ? 'Too many attempts. Please wait one minute and try again.'
        : 'Login failed. Check your email and password.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-card">
      <h2>Sign in</h2>
      {error && <p role="alert">{error}</p>}
      <form onSubmit={handleSubmit}>
        <label>Email<input type="email" value={email} onChange={e => setEmail(e.target.value)} autoComplete="email" required /></label>
        <label>Password<input type="password" value={password} onChange={e => setPassword(e.target.value)} autoComplete="current-password" required /></label>
        <button type="submit" disabled={loading}>{loading ? 'Signing in…' : 'Sign in'}</button>
      </form>
      <p>New here? <button type="button" onClick={onSwitchToRegister}>Create an account</button></p>
    </div>
  );
}
