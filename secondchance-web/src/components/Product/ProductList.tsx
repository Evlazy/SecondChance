import {useEffect, useState} from 'react';
import { productApi, type Product } from '../../api/Product/productApi';
import { IsAdminUser, parseJwt } from '../../utils/jwtHelper';

export default function ProductList() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  
  const isManager = IsAdminUser();

  const fetchProducts = async () => {
    try {
      const productArray = await productApi.getAllProducts();
      setProducts(productArray);
    } catch (err: any) {
      setError(err.response?.data?.message || '获取商品列表失败');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
  const token = sessionStorage.getItem('token');

  if (token) {
    const decryptedPayload = parseJwt(token);
  } else {
  }
    fetchProducts();
  }, []);

const handleFreeze = async (productId: string, title: string) => {
  const reason = window.prompt(`⚠️ 您正在下架商品【${title}】\n请输入下架原因（必填）：`);
  
  if (reason === null) return; 

  if (!reason.trim()) {
    alert("❌ 必须输入下架原因才能执行下架操作！");
    return;
  }

  try {
    await productApi.freezeProduct(productId, reason);
    alert('💥 下架成功，该商品已在前台隐藏！');
    
    setProducts(prev => prev.filter(p => p.id !== productId));
  } catch (err: any) {
    console.error("下架发生错误:", err);
    alert(`下架失败: ${err.response?.data?.message || '权限不足或服务器错误'}`);
  }
};

  if (loading) return <p style={{ textAlign: 'center', marginTop: '20px' }}>⏳ 正在从二手集市加载宝贝...</p>;
  if (error) return <p style={{ color: 'red', textAlign: 'center' }}>❌ 错误: {error}</p>;

  return (
    <div style={{ padding: '20px' }}>
      <h2 style={{ marginBottom: '20px' }}>🛒 今日最新上架商品 ({products.length})</h2>
      
      {products.length === 0 ? (
        <p style={{ color: '#666' }}>集市空空如也...</p>
      ) : (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(250px, 1fr))', gap: '20px' }}>
          {products.map(product => (
            <div key={product.id} style={{ border: '1px solid #e0e0e0', borderRadius: '8px', padding: '15px', display: 'flex', flexDirection: 'column', justifyContent: 'between', backgroundColor: '#fff' }}>
              <div>
                <div style={{ display: 'flex', gap: '8px', alignItems: 'center', marginBottom: '8px' }}>
                  <span style={{ backgroundColor: '#e3f2fd', color: '#0d47a1', padding: '2px 8px', borderRadius: '4px', fontSize: '12px', fontWeight: 'bold' }}>
                    ✨ {product.condition || '二手'}
                  </span>
                </div>
                <h3 style={{ margin: '0 0 10px 0', fontSize: '18px', color: '#333' }}>{product.title}</h3>
                <p style={{ color: '#666', fontSize: '14px' }}>{product.description}</p>
              </div>
              <div style={{ marginTop: '15px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <span style={{ fontSize: '20px', fontWeight: 'bold', color: '#e44d26' }}>${product.price}</span>
                
                <div style={{ display: 'flex', gap: '8px' }}>
                  <button style={{ padding: '6px 12px', backgroundColor: '#28a745', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer' }}>
                    立即联系
                  </button>

                  {/* 🎯 权限判定的灵魂所在：如果是 Admin，强行亮出红色斩杀键 */}
                  {isManager && (
                    <button 
                      onClick={() => handleFreeze(product.id, product.title)}
                      style={{ padding: '6px 12px', backgroundColor: '#dc3545', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: 'bold' }}
                    >
                      💥 违规下架
                    </button>
                  )}
                </div>

              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}