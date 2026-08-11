import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { productApi, type ProductCondition, type UpdateProductDto } from "../../api/Product/productApi";
import { categoryApi, type Categories } from "../../api/Category/categoryApi";

interface UpdateProductFormProps{
    productId: string;
    onSuccess?: () => void;
}

export const UpdateProductForm: React.FC<UpdateProductFormProps> = ({ productId, onSuccess }) => {
    const navigate = useNavigate();

    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [price, setPrice] = useState<number | ''>('');
    const [condition, setCondition] = useState<ProductCondition>("BrandNew");
    const [categoryList, setCategoryList] = useState<Categories[]>([]);
    const [selectedCategory, setSelectedCategory] = useState<string>('');
    const [errors, setError] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        console.log(selectedCategory);

        const dto: UpdateProductDto = {
            title,
            description,
            price: price === '' ? 0 : Number(price),
            condition,
            categoryId: selectedCategory,
        };

        try {
            await productApi.updateProduct(productId, dto);
            if(onSuccess){
                onSuccess();
            }else{
                navigate("/my-listing");
            }
        } catch (err: any) {
            if (err.response?.status === 401) {
                setError("You are not logged in.");
            } else if (err.response?.status === 403) {
                setError("You do not have permission to edit this product.");
            } else {
                setError(err.response?.data?.message || "Failed to update product.");
            }
        }
    };

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const data = await categoryApi.getAllCategories();
                setCategoryList(Array.isArray(data) ? data : (data as any)?.data || []);
                const existingProduct = await productApi.getProductById(productId);

                setTitle(existingProduct.title || '');
                setDescription(existingProduct.description || '');
                setPrice(existingProduct.price ?? '');
                setCondition(existingProduct.condition || 'BrandNew');
                setSelectedCategory(existingProduct.categoryId || '');
            } catch (error) {
                setError(`Failed to load categories: ${error}`);
            }
        };

        if(productId){
            fetchCategories();
        }
    }, [productId]);

    return (
        <div>
            {errors && <div style={{ color: 'red' }}>{errors}</div>}
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Title:</label>
                    <input 
                        value={title} 
                        onChange={(e) => setTitle(e.target.value)} 
                    />
                </div>

                <div>
                    <label>Description:</label>
                    <input 
                        value={description} 
                        onChange={(e) => setDescription(e.target.value)} 
                    />
                </div>

                <div>
                    <label>Price:</label>
                    <input 
                        type="number" 
                        value={price} 
                        onChange={(e) => {
                            const val = e.target.value; 
                            setPrice(val === '' ? '' : Number(val));
                        }} 
                    />
                </div>

                <div>
                    <label>Condition:</label>
                    <select
                        value={condition}
                        onChange={(e) => setCondition(e.target.value as ProductCondition)}
                    >
                        <option value="BrandNew">Brand New</option>
                        <option value="LikeNew">Like New</option>
                        <option value="GentlyUsed">Gently Used</option>
                        <option value="WellUsed">Well Used</option>
                    </select>
                </div>
                <div>
                    <label>Category:</label>
                    <select
                        value={selectedCategory}
                        onChange={(e) => setSelectedCategory(e.target.value)}
                    >
                        <option value="">Please Select Your Category</option>
                        {categoryList.map((cat) => (
                            <option key={cat.id} value={cat.id}>
                                {cat.name}
                            </option>
                        ))}
                    </select>
                </div>

                <button type="submit">Update Product</button>
            </form>
        </div>
    );
};