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

  const directLogin = async () => {
    setEmail("test2@email.com");
    setPassword("P@ssw0rd!1234567");
  }

  return (
    <div>
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
      <div>
          <button onClick={directLogin}>Use Test Account</button>
      </div>
<div>
  <h2>Welcome to SecondChance</h2>
  <p>
    <strong>SecondChance</strong> is a full-stack marketplace web application built with .NET Core, React, and PostgreSQL. 
    It enables users to buy, sell, and manage listings seamlessly with real-time state management and secure authentication.
  </p>

  <p>
    <em>Note: Since the backend is hosted on a free cloud instance, the initial log-in or request may take 15–30 seconds to wake up. Thank you for your patience!</em>
  </p>

  <p>
    <a href='https://github.com/Evlazy/SecondChance' target='_blank' rel='noopener noreferrer'>GitHub Repository</a> | 
    <a href='https://www.linkedin.com/in/jiyang-hao-a7b546289/' target='_blank' rel='noopener noreferrer'>LinkedIn Profile</a>
  </p>
</div>
    </div>
  );
}
