using System;

namespace GtkGL {
    public class Quaternion {
        double x, y, z, w;
        
		public override string ToString ()
		{
		   return (x.ToString()+","+y.ToString()+","+z.ToString()+","+w.ToString());
		}
		
		public event EventHandler Updated;
        
        public double X {
        	set { x = value; if(Updated != null) Updated(this, null); }
        	get { return x; }
        }

        public double Y {
        	set { y = value; if(Updated != null) Updated(this, null); }
        	get { return y; }
        }

        public double Z {
        	set { z = value; if(Updated != null) Updated(this, null); }
        	get { return z; }
        }

        public double W {
        	set { w = value; if(Updated != null) Updated(this, null); }
        	get { return w; }
        }
        
        static Quaternion identity = null;
        
        public static Quaternion Identity {
        		get {
	        		if (identity == null)
	        			identity = new Quaternion(1,0,0,0);	        				
	        		return identity;
    	    	}
        }
               
        const int RENORMCOUNT = 97;
        
        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
        	if(q1 == null) return null;
        		
        	if(q2 == null) return null;

        	Vector3b v1 = new Vector3b(q1.x, q1.y, q1.z);
        	Vector3b v2 = new Vector3b(q2.x, q2.y, q2.z);
        	
        	double angle = ((q1.w * q2.w) - Vector3b.Dot(v1, v2));
        	
        	Vector3b cross = Vector3b.Cross(v1, v2);
        	
        	v1 *= q2.w;
        	v2 *= q1.w;
        	
        	Quaternion result = new Quaternion(angle,
        									  (v1.x + v2.x + cross.x),
        									  (v1.y + v2.y + cross.y),
        									  (v1.z + v2.z + cross.z)
							  				  );
 
			return result;
        }
        
        
        public Quaternion(float[] q)
        {
        	this.w = (double) q[0];
        	this.x = (double) q[1];
        	this.y = (double) q[2];
        	this.z = (double) q[3];
        }
        
        public Quaternion(double[] q)
        {
        	this.w = q[0];
        	this.x = q[1];
        	this.y = q[2];
        	this.z = q[3];
        }
        
        public Quaternion(double w, double x, double y, double z)
        {
        	this.w = w;
        	this.x = x;
        	this.y = y;
        	this.z = z;
        }
        
        double GetMagnitude()
        {
        	return (w*w + x*x + y*y + z*z);
        }
        
	   	/*
	 	 * Quaternions always obey:  a^2 + b^2 + c^2 + d^2 = 1.0
	 	 * If they don't add up to 1.0, dividing by their magnitued will
	 	 * renormalize them.
	 	 *
	 	 * Note: See the following for more information on quaternions:
	 	 *
	 	 * - Shoemake, K., Animating rotation with quaternion curves, Computer
	 	 *   Graphics 19, No 3 (Proc. SIGGRAPH'85), 245-254, 1985.
	 	 * - Pletinckx, D., Quaternion calculus as a basic tool in computer
	 	 *   graphics, The Visual Computer 5, 2-13, 1989.
	 	 */

        public Quaternion Normalize()
        {
		    double mag = GetMagnitude();

		    w /= mag;
		    x /= mag;
		    y /= mag;
		    z /= mag;
		    
		    return this;
		}

		public GtkGL.EulerRotation ToEulerRotation()
		{
			double heading;
			double attitude;
			double bank;
		    
			double magnitude = GetMagnitude(); // 1.0 if normalised, otherwise is correction factor
	        double test = x*y + z*w;
	        
		    if (test > 0.499 * magnitude) { // singularity at north pole
		      heading = 2 * Math.Atan2(x,w);
		      attitude = Math.PI/2;
		      bank = 0;
			  Console.WriteLine("Eep!  Singularity at north pole!");
		      return new GtkGL.EulerRotation(heading, attitude, bank);
		    }
		    
		    if (test < -0.499 * magnitude) { // singularity at south pole
		      heading = -2 * Math.Atan2(x,w);
		      attitude = - Math.PI/2;
		      bank = 0;
		      Console.WriteLine("Eep!  Singularity at south pole!");
		      return new GtkGL.EulerRotation(heading, attitude, bank);
		    }
		    
		    heading = Math.Atan2(2*y*w-2*x*z , 1 - 2*y*y - 2*z*z);
		    attitude = Math.Asin(2*test/magnitude);
		    bank = Math.Atan2(2*x*w-2*y*z , 1 - 2*x*x - 2*z*z);

			return new GtkGL.EulerRotation(heading, attitude, bank);
		}
		
		public GtkGL.TransformationMatrix ToTransMatrix()
		{
			double[] transMatrix = new double[16];
			
	        transMatrix[4 * 0 + 0] = 1.0 - 2.0f * (y * y + z * z);
		    transMatrix[4 * 0 + 1] = 2.0 * (x * y - z * w);
		    transMatrix[4 * 0 + 2] = 2.0 * (z * x + y * w);

		    transMatrix[4 * 1 + 0] = 2.0 * (x * y + z * w);
		    transMatrix[4 * 1 + 1]= 1.0 - 2.0f * (z * z + x * x);
		    transMatrix[4 * 1 + 2] = 2.0 * (y * z - x * w);

		    transMatrix[4 * 2 + 0] = 2.0 * (z * x - y * w);
	    	transMatrix[4 * 2 + 1] = 2.0 * (y * z + x * w);
		    transMatrix[4 * 2 + 2] = 1.0 - 2.0f * (y * y + x * x);

			transMatrix[3] = transMatrix[7] = transMatrix[11] =
			transMatrix[12] = transMatrix[13] = transMatrix[14] = 0.0;
			
			transMatrix[15] = 1.0;
		    
		    return new GtkGL.TransformationMatrix(transMatrix);
		}
    }   
}