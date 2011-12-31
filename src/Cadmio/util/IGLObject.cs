using System;
using GtkGL;

namespace GtkGL {
    
    public interface IGLObject {
    	// An event handler to notify folks of updates
		event EventHandler Updated;    
    
    	// A hook for initialization tasks
        void Init();
        
        // Draw the object
        bool Draw();
        
        int glID { get; }
        bool Selected { get; set; }
        
        // Translation (movement) methods
        void Translate(float x, float y, float z);
        void Translate(double x, double y, double z);
        
        void Scale(double[] factor);
        void Scale(double xFactor, double yFactor, double zFactor);
        void Scale(double factor);

  		// Rotate the object by passing a quaternion
  		void Rotate(GtkGL.Quaternion quat);
  		
  		// Rotate the object by passing an Euler rotation
  		void Rotate(GtkGL.EulerRotation eRot);
  		
  		// Reset the rotation to the identity, and fire the 'Updated' handlers if 'doUpdate'
  		void ResetRotation(bool doUpdate);
  		
  		// Reset the rotation to the identity
  		void ResetRotation();
  			
  		// Return the rotation as Euler angles
  		EulerRotation GetEulerRotation();
  		
  		// Return the rotation as a Quaternion
  		Quaternion GetQuaternion();
  		
  		// Return the TransformationMatrix
  		TransformationMatrix GetTransformationMatrix();
    }   
}