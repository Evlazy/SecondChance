import React, {useState} from "react";
import { productApi } from "../../api/Product/productApi";
import { useNavigate } from "react-router-dom";

interface DeleteProductProps{
    productId: string;
    onSuccess?: () => void;
    onCancel?:() => void;
}

export const DeleteProduct:React.FC<DeleteProductProps> = ({productId, onSuccess, onCancel}) => {
    const navigate = useNavigate();
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handelDelete = async () => {

        setLoading(true);
        setError('');
        try{
            await productApi.deleteProduct(productId);
            if(onSuccess){
                onSuccess();
            }else{
                navigate("/my-listing");
            }
        } catch(err: any){
            if(err.response?.status === 401){
                setError("You are not logged in.");
            }else if(err.response?.status === 403){
                setError("You do not have permission to delete this product.");
            } else{
                setError(err.response?.data?.message || "Failed to delete product.");
            }
        }finally{
            setLoading(false);
        }  
    };

    return(
        <div>
            {error && <p style={{color:'red'}}>{error}</p>}
            <h2>Are You Sure You Want To Delete This Product?</h2>
            <button onClick={handelDelete} disabled={loading}>
                {loading ? "Deleting..." : "Yes"}
            </button>
            <button onClick={onCancel} type="button" disabled={loading}>No</button>
        </div>
    )
}