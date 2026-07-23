import React, { useState, useEffect } from 'react';
import { productApi , type ProductCondition} from '../../api/Product/productApi';
import { getAuthenticatedUser } from '../../utils/jwtHelper';
import { categoryApi, type Categories } from '../../api/Category/categoryApi';

export default function CreateProduct() {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [price, setPrice] = useState<number>(0);
  const [condition, setCondition] = useState<ProductCondition>('BrandNew'); 
  const [categoryId, setCategoryId] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [categories, setCategories] = useState<Categories[]>([]);

  useEffect(() => {
    const currentUser = getAuthenticatedUser();
    if (!currentUser) {
      window.location.href = "/login";
      return;
    }

const fetchCategories = async () => {
    try {
      const data = await categoryApi.getAllCategories();
      setCategories(data);
    } catch (err: any) {
    }
  };

  fetchCategories();
}, []);

  console.log("Categories: ", categories);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault(); // Prevents full-page web reloads
    setError('');
    setLoading(true);

    try {
      await productApi.createProduct({
        title,
        description,
        price,
        condition,
        categoryId,
        sellerId: "" 
      });

      alert('Product listed successfully!');
      window.location.href = '/'; 
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to list product. Please review your inputs.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ maxWidth: '500px', margin: '40px auto', padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
      <h2>List a New Product</h2>

      {error && (
        <div style={{ color: 'red', marginBottom: '15px', fontWeight: 'bold' }}>
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px' }}>Title</label>
          <input 
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box' }}
          />
        </div>

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px' }}>Description</label>
          <textarea 
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            required
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box', minHeight: '80px' }}
          />
        </div>

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px' }}>Price ($)</label>
          <input 
            type="number"
            min="0"
            value={price}
            onChange={(e) => setPrice(Number(e.target.value))} 
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box' }}
          />
        </div>

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px' }}>Condition</label>
          <select 
            value={condition} 
            onChange={(e) => setCondition(e.target.value as ProductCondition)}
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box' }}
          >
            <option value="BrandNew">Brand New</option>
            <option value="LikeNew">Like New</option>
            <option value="GentlyUsed">Gently Used</option>
            <option value="WellUsed">Well Used</option>
          </select>
        </div>

        <div style={{ marginBottom: '20px' }}>
          <label style={{ display: 'block', marginBottom: '5px' }}>Category Code/ID</label>
          <select
            value={categoryId}
            onChange={(e) => setCategoryId(e.target.value)}
            required
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box' }}
          >
            <option value="">-- Select a Category --</option>
            {categories.map((category) => (
              <option key={category.id} value={category.id}>
                {category.name}
              </option>
            ))}
          </select>
        </div>

        <button 
          type="submit" 
          disabled={loading}
          style={{ width: '100%', padding: '10px', backgroundColor: '#28a745', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer' }}
        >
          {loading ? 'Publishing Item...' : 'List Product'}
        </button>
      </form>
    </div>
  );
}