import { useEffect, useState } from 'react';
import { productApi, type Product, type ProductImageResponse } from '../../api/Product/productApi';
import { IsAdminUser, parseJwt } from '../../utils/jwtHelper';
import { UploadProductImageModal } from './UploadProductImage';

const API_BASE_URL = "https://secondchance-api-a8cb.onrender.com";

export default function ProductList() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  
  const [selectedProductId, setSelectedProductId] = useState<string | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);

  const isManager = IsAdminUser();

  const fetchProducts = async () => {
    try {
      const productArray = await productApi.getAllProducts();
      setProducts(productArray);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to fetch product list.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const token = sessionStorage.getItem('token');

    if (token) {
      parseJwt(token);
    }

    fetchProducts();
  }, []);

  const handleFreeze = async (productId: string, title: string) => {
    const reason = window.prompt(`You are taking down item: [${title}]\nPlease enter a reason (required):`);
    
    if (reason === null) return; 

    if (!reason.trim()) {
      alert("A reason is required to take down this item!");
      return;
    }

    try {
      await productApi.freezeProduct(productId, reason);
      alert('Item taken down successfully! It is now hidden from the store.');
      
      setProducts(prev => prev.filter(p => p.id !== productId));
    } catch (err: any) {
      console.error("Error during item takedown:", err);
      alert(`Failed to take down item: ${err.response?.data?.message || 'Insufficient permissions or server error.'}`);
    }
  };

  const handleOpenUploadModal = (productId: string) => {
    setSelectedProductId(productId);
    setIsModalOpen(true);
  };

  const handleImageUploaded = (newImage: ProductImageResponse) => {
    setProducts(prevProducts =>
      prevProducts.map(p => {
        if (p.id === newImage.productId) {
          const currentImages = p.images || [];
          return {
            ...p,
            images: [...currentImages, newImage]
          };
        }
        return p;
      })
    );
  };

const getImageUrl = (url: string) => {
  if (!url) return '';

  if (url.includes('localhost:')) {
    const relativePath = url.substring(url.indexOf('/uploads'));
    return `${API_BASE_URL}${relativePath}`;
  }

  if (url.startsWith('http://') || url.startsWith('https://')) {
    return url;
  }

  const formattedPath = url.startsWith('/') ? url : `/${url}`;
  return `${API_BASE_URL}${formattedPath}`;
};

  if (loading) return <p style={{ textAlign: 'center', marginTop: '20px' }}>⏳ Loading items from SecondChance Market...</p>;
  if (error) return <p style={{ color: 'red', textAlign: 'center' }}>❌ Error: {error}</p>;

  return (
    <div style={{ padding: '20px' }}>
      <h2 style={{ marginBottom: '20px' }}>🛒 Today's Latest Items ({products.length})</h2>
      
      {products.length === 0 ? (
        <p style={{ color: '#666' }}>No items available right now...</p>
      ) : (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))', gap: '20px' }}>
          {products.map(product => {
            // Added explicit ProductImageResponse type for img parameter
            const mainImage = product.images?.find((img: ProductImageResponse) => img.isMain) || product.images?.[0];

            return (
              <div 
                key={product.id} 
                style={{ 
                  border: '1px solid #e0e0e0', 
                  borderRadius: '8px', 
                  padding: '15px', 
                  display: 'flex', 
                  flexDirection: 'column', 
                  justifyContent: 'space-between',
                  backgroundColor: '#fff',
                  overflow: 'hidden' 
                }}
              >
                <div>
                  <div style={{ width: '100%', height: '180px', backgroundColor: '#f5f5f5', borderRadius: '6px', overflow: 'hidden', marginBottom: '12px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    {mainImage ? (
                      <img 
                        src={getImageUrl(mainImage.imageUrl)} 
                        alt={product.title} 
                        style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                      />
                    ) : (
                      <span style={{ color: '#aaa', fontSize: '14px' }}>📷 No Image Available</span>
                    )}
                  </div>

                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '8px' }}>
                    <span style={{ backgroundColor: '#e3f2fd', color: '#0d47a1', padding: '2px 8px', borderRadius: '4px', fontSize: '12px', fontWeight: 'bold' }}>
                      {product.condition || 'Used'}
                    </span>

                    <button
                      onClick={() => handleOpenUploadModal(product.id)}
                      style={{ background: 'none', border: '1px dashed #0066cc', color: '#0066cc', borderRadius: '4px', padding: '2px 6px', fontSize: '12px', cursor: 'pointer' }}
                    >
                      + Add Photo
                    </button>
                  </div>

                  <h3 style={{ margin: '0 0 8px 0', fontSize: '18px', color: '#333' }}>{product.title}</h3>
                  <p style={{ color: '#666', fontSize: '14px', margin: '0 0 10px 0' }}>{product.description}</p>
                </div>

                <div style={{ marginTop: '15px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <span style={{ fontSize: '20px', fontWeight: 'bold', color: '#e44d26' }}>${product.price}</span>
                  
                  <div style={{ display: 'flex', gap: '8px' }}>
                    <button style={{ padding: '6px 12px', backgroundColor: '#28a745', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer' }}>
                      Contact Seller
                    </button>

                    {isManager && (
                      <button 
                        onClick={() => handleFreeze(product.id, product.title)}
                        style={{ padding: '6px 12px', backgroundColor: '#dc3545', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: 'bold' }}
                      >
                        Take Down
                      </button>
                    )}
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      )}

      {selectedProductId && (
        <UploadProductImageModal
          productId={selectedProductId}
          isOpen={isModalOpen}
          onClose={() => {
            setIsModalOpen(false);
            setSelectedProductId(null);
          }}
          onImageUploaded={handleImageUploaded}
        />
      )}
    </div>
  );
}