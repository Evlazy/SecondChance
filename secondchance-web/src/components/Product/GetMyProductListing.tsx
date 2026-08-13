import React, { useEffect, useState } from "react";
import { productApi, type Product } from "../../api/Product/productApi";
import { getAuthenticatedUser } from "../../utils/jwtHelper";
import { UpdateProductForm } from "./UpdateProduct";
import { DeleteProduct } from './DeleteProduct';


export default function MyListingProduct() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [editingProductId, setEditingProductId] = useState<string | null>(null);
  const [deleteProductId, setDeleteProductId] = useState<string | null>(null);

  const user = getAuthenticatedUser();

  const fetchProducts = async () => {
    if (!user?.id) return;

    setLoading(true);
    setError(null);
    try {
      const listingProduct = await productApi.getMyListingProducts();

      let allListings: Product[] = [];
      if (Array.isArray(listingProduct)) {
        allListings = listingProduct;
      } else if (listingProduct && Array.isArray((listingProduct as any).data)) {
        allListings = (listingProduct as any).data;
      }

      setProducts(allListings);
      console.log(allListings);
    } catch (err: any) {
      setError("Failed to load your listings. Please try again later.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!user || !user.id) {
      window.location.href = "/login";
      return;
    }
    fetchProducts();
  }, []);

if (editingProductId) {
    return (
      <div style={styles.container}>
        <button 
          type="button" 
          onClick={() => setEditingProductId(null)}
          style={{ marginBottom: '16px' }}
        >
          ← Back to My Listings
        </button>
        <UpdateProductForm productId={editingProductId}
          onSuccess={() => {
            setEditingProductId(null);
            fetchProducts();
          }} />
      </div>
    );
  }

  if(deleteProductId){
    return(
      <div>
        <button
          type="button"
          onClick={() => setDeleteProductId(null)}
          />
          <DeleteProduct productId={deleteProductId}
            onSuccess={() => {
              setDeleteProductId(null);
              fetchProducts();
            }} 
            onCancel={() => setDeleteProductId(null)}
            />
      </div>
    )
  }

  return (
    <div style={styles.container}>
      <header style={styles.header}>
        <h1 style={styles.title}>My Listed Products</h1>
      </header>

      {error && <div style={styles.errorBanner}>{error}</div>}

      {loading ? (
        <div style={styles.stateMessage}>Loading your listings...</div>
      ) : products.length === 0 ? (
        <div style={styles.stateMessage}>No Products Listed Yet</div>
      ) : (
        <div style={styles.grid}>
          {products.map((pro) => {
            const displayImageUrl = pro.images?.[0]?.imageUrl;

            return (
              <div key={pro.id} style={styles.card}>
                {displayImageUrl && (
                  <img
                    src={displayImageUrl}
                    alt={pro.title}
                    style={styles.image}
                  />
                )}
                <div style={styles.cardBody}>
                  <div style={styles.cardHeader}>
                    <h3 style={styles.productTitle}>{pro.title}</h3>
                  </div>

                  {pro.condition && (
                    <div style={styles.conditionTag}>
                      Condition: {pro.condition}
                    </div>
                  )}

                  {pro.description && (
                    <p style={styles.description}>{pro.description}</p>
                  )}

                  <div style={styles.cardFooter}>
                    <span style={styles.price}>${pro.price ?? 0}</span>
                    <button
                        type="button"
                        onClick={() => setEditingProductId(pro.id)}
                        >Edit
                        </button>
                  </div>

                  <div style={styles.cardFooter}>
                    <button
                        type="button"
                        onClick={() => setDeleteProductId(pro.id)}
                        >Delete
                        </button>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  container: {
    maxWidth: "1000px",
    margin: "0 auto",
    padding: "24px 16px",
    fontFamily: "system-ui, -apple-system, sans-serif",
  },
  header: {
    marginBottom: "24px",
    borderBottom: "1px solid #eaeaea",
    paddingBottom: "12px",
  },
  title: {
    fontSize: "24px",
    fontWeight: "600",
    color: "#111827",
    margin: 0,
  },
  errorBanner: {
    padding: "12px 16px",
    backgroundColor: "#fef2f2",
    color: "#dc2626",
    borderRadius: "6px",
    border: "1px solid #fecaca",
    marginBottom: "16px",
  },
  stateMessage: {
    textAlign: "center",
    padding: "48px 0",
    color: "#6b7280",
    fontSize: "16px",
  },
  grid: {
    display: "grid",
    gridTemplateColumns: "repeat(auto-fill, minmax(280px, 1fr))",
    gap: "20px",
  },
  card: {
    border: "1px solid #e5e7eb",
    borderRadius: "8px",
    overflow: "hidden",
    backgroundColor: "#ffffff",
    boxShadow: "0 1px 3px rgba(0,0,0,0.05)",
    display: "flex",
    flexDirection: "column",
  },
  image: {
    width: "100%",
    height: "180px",
    objectFit: "cover",
  },
  cardBody: {
    padding: "16px",
    display: "flex",
    flexDirection: "column",
    flex: 1,
  },
  cardHeader: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "flex-start",
    marginBottom: "8px",
    gap: "8px",
  },
  productTitle: {
    margin: 0,
    fontSize: "16px",
    fontWeight: "600",
    color: "#1f2937",
  },
  statusBadge: {
    backgroundColor: "#eff6ff",
    color: "#2563eb",
    fontSize: "12px",
    fontWeight: "500",
    padding: "2px 8px",
    borderRadius: "12px",
    textTransform: "capitalize",
    whiteSpace: "nowrap",
  },
  conditionTag: {
    fontSize: "12px",
    color: "#6b7280",
    marginBottom: "8px",
  },
  description: {
    fontSize: "14px",
    color: "#4b5563",
    margin: "0 0 16px 0",
    lineHeight: "1.4",
  },
  cardFooter: {
    marginTop: "auto",
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    paddingTop: "12px",
    borderTop: "1px solid #f3f4f6",
  },
  price: {
    fontSize: "18px",
    fontWeight: "700",
    color: "#059669",
  },
};