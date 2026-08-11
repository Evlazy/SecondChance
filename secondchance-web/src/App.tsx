import React, { useState } from 'react';
import { BrowserRouter, Routes, Route, Link, useNavigate, Navigate } from 'react-router-dom';

import Login from './components/Auth/Login';
import Register from './components/Auth/Register';
import ProductList from './components/Product/ProductList';
import MyListingProduct from './components/Product/GetMyProductListing';

import { ProductDetail } from './components/Product/GetProductById';
import CreateProduct from './components/Product/CreateProduct';
import { MyOrders } from './components/Purchase/MyOrders';

function AppContent() {
  const hasToken = !!sessionStorage.getItem('token');
  const [view, setView] = useState<'login' | 'register'>('login');
  const navigate = useNavigate();

  const handleLogout = () => {
    sessionStorage.removeItem('token');
    alert("Logout successfully!");
    navigate('/');
    window.location.reload();
  };

  return (
    <div style={{ fontFamily: 'sans-serif', backgroundColor: '#f8f9fa', minHeight: '100vh', padding: '20px' }}>
      <header style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', backgroundColor: '#fff', padding: '10px 20px', borderRadius: '8px', boxShadow: '0 2px 4px rgba(0,0,0,0.05)' }}>
        <Link to="/" style={{ textDecoration: 'none' }}>
          <h1 style={{ margin: 0, fontSize: '24px', color: '#007bff' }}>🎪 SecondChance Marketplace</h1>
        </Link>
        
        {hasToken && (
          <div style={{ display: 'flex', alignItems: 'center', gap: '15px' }}>
            <Link to="/" style={styles.navLink}>
              Browse Products
            </Link>
            <Link to="/orders" style={styles.navLink}>
              My Orders
            </Link>
            <Link to="/create-product" style={styles.navLink}>
              List a Product
            </Link>
            <Link to="/my-listing" style={styles.navLink}>
              My Listings
            </Link>
            <button onClick={handleLogout} style={styles.logoutButton}>
              Log out
            </button>
          </div>
        )}
      </header>

      <main style={{ marginTop: '20px' }}>
        {!hasToken ? (
          view === 'login' ? (
            <Login onSwitchToRegister={() => setView('register')} />
          ) : (
            <Register onSwitchToLogin={() => setView('login')} />
          )
        ) : (
          <Routes>
            <Route path="/" element={<ProductList />} />
            <Route path="/products/:id" element={<ProductDetail />} />
            <Route path="/orders" element={<MyOrders />} />
            <Route path="/create-product" element={<CreateProduct />} />
            <Route path="/my-listing" element={<MyListingProduct />} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        )}
      </main>
    </div>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <AppContent />
    </BrowserRouter>
  );
}

const styles: Record<string, React.CSSProperties> = {
  navLink: {
    padding: '8px 12px',
    color: '#007bff',
    textDecoration: 'none',
    fontSize: '16px',
    fontWeight: '500',
    cursor: 'pointer',
  },
  logoutButton: {
    padding: '8px 15px',
    backgroundColor: '#dc3545',
    color: 'white',
    border: 'none',
    borderRadius: '4px',
    cursor: 'pointer',
    fontWeight: '600',
  }
};