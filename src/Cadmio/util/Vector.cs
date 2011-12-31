namespace GtkGL {

    public class Vector3b {
    	public double x, y, z;
        
        public Vector3b(double x, double y, double z) {
        	this.x = x;
        	this.y = y;
        	this.z = z;
        }
        
        public Vector3b(double[] v) {
        	if(v == null || v.Length < 3)
        		this.x = this.y = this.z = 0.0;
        	else{
        		this.x = v[0];
        		this.y = v[1];
        		this.z = v[2];
        	}
        }
        
        public static Vector3b Cross(Vector3b v1, Vector3b v2)
        {
        	return new Vector3b( (v1.y * v2.z) - (v1.z * v2.y),
        					   (v1.z * v2.x) - (v1.x * v2.z),
        					   (v1.x * v2.y) - (v1.y * v2.x)
        					 );									 
        }
        
        public static Vector3b operator +(Vector3b v1, Vector3b v2)
        {
        	return new Vector3b(v1.x + v2.x,
        					  v1.y + v2.y,
        					  v1.z + v2.z
        					 );
        }
        
        public static Vector3b operator *(Vector3b v1, double factor)
        {
        	Vector3b result = new Vector3b( v1.x * factor,
        								v1.y * factor,
        								v1.z * factor
        							   );
        	return result;
        }
        
        public static double Dot(Vector3b v1, Vector3b v2)
        {
        	return (v1.x * v2.x) + (v1.y * v2.y) + (v1.z * v2.z);
        }  
    }   
}