import React, {useState} from 'react';
import { authApi } from '../../api/Auth/authApi';

interface LoginProps{
  onSwitchToRegister: () => void;
}

export default function Login({onSwitchToRegister}: LoginProps){
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

        try{
            const data = await authApi.login(email, password);

            sessionStorage.setItem('token', data.token);

            alert('Login success');
            window.location.href = '/';
        }
        catch(err:any){
            setError(err.response?.data?.message || 'Login failed, please check your email or password');
        }
        finally{
            setLoading(false);
        }
    };

    
return (
    <div style={{ maxWidth: '400px', margin: '50px auto', padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
      <h2>🔑 SecondChance Login</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      
      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px' }}>Email:</label>
          <input 
            type="email" 
            value={email} 
            onChange={(e) => setEmail(e.target.value)} 
            required 
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box' }}
          />
        </div>
        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px' }}>Password:</label>
          <input 
            type="password" 
            value={password} 
            onChange={(e) => setPassword(e.target.value)} 
            required 
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box' }}
          />
        </div>
        <button type="submit" disabled={loading} style={{ width: '100%', padding: '10px', backgroundColor: '#007bff', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer' }}>
          {loading ? 'Logging you in...' : 'Login'}
        </button>

        <p style={{marginTop: '15px', textAlign:'center'}}>
          Don't have an account?{''}
          <button 
              type='button'
              onClick={onSwitchToRegister}
              style={{ background: 'none', border: 'none', color: '#007bff', cursor: 'pointer', textDecoration: 'underline' }}
          >
            Register here
          </button>  
        </p>
      </form>
    </div>
  );
}
