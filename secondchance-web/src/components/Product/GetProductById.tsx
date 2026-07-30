import { productApi, type Product } from "../../api/Product/productApi";
import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';

const API_BASE_URL = "https://secondchance-api-a8cb.onrender.com/api";

export const ProductDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedImage, setSelectedImage] = useState<string | null>(null);

  const getImageUrl = (url: string) => {
    if (!url) return '';
    if (url.startsWith('http://') || url.startsWith('https://')) {
      return url;
    }
    const formattedPath = url.startsWith('/') ? url : `/${url}`;
    return `${API_BASE_URL}${formattedPath}`;
  };

  useEffect(() => {
    if (!id) return;

    const loadProduct = async () => {
      try {
        setLoading(true);
        const data = await productApi.getProductById(id);
        setProduct(data);

        if (data?.images && data.images.length > 0) {
          const main = data.images.find((img: any) => img.isMain || img.IsMain) || data.images[0];
          setSelectedImage(main.imageUrl);
        }
      } catch (err: any) {
        if (err.response?.status === 404) {
          setError('Product not found.');
        } else {
          setError('Failed to fetch product details.');
        }
      } finally {
        setLoading(false);
      }
    };

    loadProduct();
  }, [id]);

  if (loading) return <div style={{ padding: '20px' }}>Loading product details...</div>;
  if (error) return <div style={{ padding: '20px', color: 'red' }}>{error}</div>;
  if (!product) return <div style={{ padding: '20px' }}>No product data found.</div>;

  return (
    <div style={{ maxWidth: '1000px', margin: '0 auto', padding: '20px' }}>
      <button 
        onClick={() => navigate(-1)} 
        style={{ marginBottom: '20px', padding: '8px 16px', cursor: 'pointer' }}
      >
        ← Back to List
      </button>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '30px' }}>
        
        <div>
          <div style={{
            width: '100%',
            height: '350px',
            backgroundColor: '#f8f9fa',
            borderRadius: '8px',
            overflow: 'hidden',
            border: '1px solid #e0e0e0',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            marginBottom: '15px'
          }}>
            {selectedImage ? (
              <img 
                src={getImageUrl(selectedImage)} 
                alt={product.title} 
                style={{ width: '100%', height: '100%', objectFit: 'contain' }}
              />
            ) : (
              <span style={{ color: '#aaa' }}>📷 No Image Available</span>
            )}
          </div>


          {product.images && product.images.length > 1 && (
            <div style={{ display: 'flex', gap: '10px', overflowX: 'auto', paddingBottom: '5px' }}>
              {product.images.map((img: any) => {
                const isSelected = selectedImage === img.imageUrl;
                return (
                  <button
                    key={img.id || img.imageUrl}
                    onClick={() => setSelectedImage(img.imageUrl)}
                    style={{
                      border: isSelected ? '2px solid #0066cc' : '1px solid #ddd',
                      borderRadius: '6px',
                      padding: 0,
                      cursor: 'pointer',
                      background: 'none',
                      opacity: isSelected ? 1 : 0.7,
                      overflow: 'hidden',
                      width: '70px',
                      height: '70px',
                      flexShrink: 0
                    }}
                  >
                    <img 
                      src={getImageUrl(img.imageUrl)} 
                      alt="Thumbnail" 
                      style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                    />
                  </button>
                );
              })}
            </div>
          )}
        </div>

        <div>
          <h1 style={{ margin: '0 0 10px 0', fontSize: '28px' }}>{product.title}</h1>
          
          <p style={{ fontSize: '24px', fontWeight: 'bold', color: '#e44d26', margin: '0 0 15px 0' }}>
            ${product.price}
          </p>

          <div style={{ marginBottom: '20px' }}>
            <span style={{ 
              backgroundColor: '#e3f2fd', 
              color: '#0d47a1', 
              padding: '4px 10px', 
              borderRadius: '4px', 
              fontSize: '14px', 
              fontWeight: 'bold' 
            }}>
              Condition: {product.condition || 'Used'}
            </span>
          </div>

          <h3>Description</h3>
          <p style={{ color: '#444', lineHeight: '1.6' }}>{product.description}</p>

          <button style={{
            marginTop: '20px',
            padding: '12px 24px',
            backgroundColor: '#28a745',
            color: '#fff',
            border: 'none',
            borderRadius: '6px',
            fontSize: '16px',
            cursor: 'pointer',
            fontWeight: 'bold'
          }}>
            Contact Seller
          </button>
        </div>

      </div>
    </div>
  );
};