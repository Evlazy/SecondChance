// import React from 'react';
import Login from './components/Auth/Login';
import ProductList from './components/Product/ProductList'; 
import CreateProduct from './components/Product/CreateProduct';
import Register from './components/Auth/Register';
import { MyOrders } from './components/Purchase/MyOrders';
import React, { useState } from 'react';


type ActiveView = 'products' | 'orders' | 'createProduct';

function App() {
  const hasToken = !!sessionStorage.getItem('token');

  const [activeView, setActiveView] = useState<ActiveView>('products');
  const [view, setView] = useState<'login' | 'register'>('login');

  const handleLogout = () => {
    sessionStorage.removeItem('token');
    alert("Logout successfully!");
    window.location.reload();
  };

  const renderView = () => {
    switch (activeView){
      case 'products' :
        return <ProductList />;
      case 'orders' :
        return <MyOrders />;
      case 'createProduct' :
        return <CreateProduct />;
    }
  }

  return (
    <div style={{ fontFamily: 'sans-serif', backgroundColor: '#f8f9fa', minHeight: '100vh', padding: '20px' }}>
      
      <header style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', backgroundColor: '#fff', padding: '10px 20px', borderRadius: '8px', boxShadow: '0 2px 4px rgba(0,0,0,0.05)' }}>
        <h1 style={{ margin: 0, fontSize: '24px', color: '#007bff' }}>🎪 SecondChance Marketplace</h1>
        
        {hasToken && (
          <div style={{ display: 'flex', alignItems: 'center', gap: '15px' }}>
            <button 
              onClick={() => setActiveView('products')} 
              style={{
                ...styles.navButton,
                color: activeView === 'products' ? '#007bff' : '#666',
                fontWeight: activeView === 'products' ? '700' : '500'
              }}
            >
              Browse Products
            </button>
            <button 
              onClick={() => setActiveView('orders')} 
              style={{
                ...styles.navButton,
                color: activeView === 'orders' ? '#007bff' : '#666',
                fontWeight: activeView === 'orders' ? '700' : '500'
              }}
            >
              My Orders
            </button>
            <button 
              onClick={() => setActiveView('createProduct')} 
              style={{
                ...styles.navButton,
                color: activeView === 'createProduct' ? '#007bff' : '#666',
                fontWeight: activeView === 'createProduct' ? '700' : '500'
              }}
            >
              List a Product
            </button>
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
          <div>
            {renderView()}
          </div>
        )}
      </main>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  navButton: {
    padding: '8px 12px',
    background: 'none',
    border: 'none',
    fontSize: '16px',
    cursor: 'pointer',
    transition: 'color 0.2s',
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

export default App;