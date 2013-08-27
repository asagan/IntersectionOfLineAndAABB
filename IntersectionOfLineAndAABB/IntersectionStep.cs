using System;
using System.Collections.Generic;
using System.Text;
using SimioAPI;
using SimioAPI.Extensions;

namespace IntersectionOfLineAndAABB
{
    public class IntersectionOfLineAndAABBStepDefinition : IStepDefinition
    {
        #region IStepDefinition Members

        /// <summary>
        /// Property returning the full name for this type of step. The name should contain no spaces. 
        /// </summary>
        public string Name
        {
            get { return "IntersectionStep"; }
        }

        /// <summary>
        /// Property returning a short description of what the step does.  
        /// </summary>
        public string Description
        {
            get { return "Checks if an input line intersects a bounding box.  Exits through exit1 if true, exit2 if false."; }
        }

        /// <summary>
        /// Property returning an icon to display for the step in the UI. 
        /// </summary>
        public System.Drawing.Image Icon
        {
            get { return null; }
        }

        /// <summary>
        /// Property returning a unique static GUID for the step.  
        /// </summary>
        public Guid UniqueID
        {
            get { return MY_ID; }
        }
        static readonly Guid MY_ID = new Guid("{02b6a027-6d7d-4b55-b268-3a4c2e1b6be7}");

        /// <summary>
        /// Property returning the number of exits out of the step. Can return either 1 or 2. 
        /// </summary>
        public int NumberOfExits
        {
            get { return 2; }
        }

        /// <summary>
        /// Method called that defines the property schema for the step.
        /// </summary>
        public void DefineSchema(IPropertyDefinitions schema)
        {
            // Example of how to add a property definition to the step.
            
            //Inputs
            IPropertyDefinition Ox, Oy, Oz;
            IPropertyDefinition Rx, Ry, Rz;
            IPropertyDefinition Rlength;
            IPropertyDefinition B0x, B0y, B0z;
            IPropertyDefinition B1x, B1y, B1z;

            Ox = schema.AddExpressionProperty("Ox", "0.0");
            Oy = schema.AddExpressionProperty("Oy", "0.0");
            Oz = schema.AddExpressionProperty("Oz", "0.0");
            Rx = schema.AddExpressionProperty("Rx", "1.0");
            Ry = schema.AddExpressionProperty("Ry", "1.0");
            Rz = schema.AddExpressionProperty("Rz", "1.0");
            Rlength = schema.AddExpressionProperty("Rlength", "1.0");

            Ox.DisplayName = "Line Origin X";
            Oy.DisplayName = "Line Origin Y";
            Oz.DisplayName = "Line Origin Z";
            Rx.DisplayName = "Line X Vector";
            Ry.DisplayName = "Line Y Vector";
            Rz.DisplayName = "Line Z Vector";
            Rlength.DisplayName = "Length of the line";

            Ox.Description = "The X coordinate of the beginning of the line";
            Oy.Description = "The Y coordinate of the beginning of the line";
            Oz.Description = "The Z coordinate of the beginning of the line";
            Rx.Description = "The X component of the vector describing the lines direction";
            Ry.Description = "The Y component of the vector describing the lines direction";
            Rz.Description = "The Z component of the vector describing the lines direction";
            Rlength.Description = "The length of the line to be tested";

            Ox.Required = true;
            Oy.Required = true;
            Oz.Required = true;
            Rx.Required = true;
            Ry.Required = true;
            Rz.Required = true;
            Rlength.Required = true;

            B0x = schema.AddExpressionProperty("B0x", "0.0");
            B0y = schema.AddExpressionProperty("B0y", "0.0");
            B0z = schema.AddExpressionProperty("B0z", "0.0");

            B0x.DisplayName = "Box Minimum X";
            B0y.DisplayName = "Box Minimum Y";
            B0z.DisplayName = "Box Minimum Z";

            B0x.Description = "The minimum X coordinate of the bounding box";
            B0y.Description = "The minimum Y coordinate of the bounding box";
            B0z.Description = "The minimum Z coordinate of the bounding box";
 
            B0x.Required = true;
            B0y.Required = true;
            B0z.Required = true;

            B1x = schema.AddExpressionProperty("B1x", "0.0");
            B1y = schema.AddExpressionProperty("B1y", "0.0");
            B1z = schema.AddExpressionProperty("B1z", "0.0");

            B1x.DisplayName = "Box Maximum X";
            B1y.DisplayName = "Box Maximum Y";
            B1z.DisplayName = "Box Maximum Z";

            B1x.Description = "The maximum X coordinate of the bounding box";
            B1y.Description = "The maximum Y coordinate of the bounding box";
            B1z.Description = "The maximum Z coordinate of the bounding box";

            B1x.Required = true;
            B1y.Required = true;
            B1z.Required = true;


            //outputs
            IPropertyDefinition SteeringForceX, SteeringForceY, SteeringForceZ;
            IPropertyDefinition distanceFromIntersection;
            SteeringForceX = schema.AddStateProperty("SteeringForceX");
            SteeringForceY = schema.AddStateProperty("SteeringForceY");
            SteeringForceZ = schema.AddStateProperty("SteeringForceZ");
            SteeringForceX.DisplayName = "State X Component Steering Force";
            SteeringForceY.DisplayName = "State Y Component Steering Force";
            SteeringForceZ.DisplayName = "State Z Component Steering Force";
            SteeringForceX.Description = "The state to store the x component of the steering force";
            SteeringForceY.Description = "The state to store the y component of the steering force";
            SteeringForceZ.Description = "The state to store the z component of the steering force";
            SteeringForceX.Required = false;
            SteeringForceY.Required = false;
            SteeringForceZ.Required = false;

            distanceFromIntersection = schema.AddStateProperty("DistanceFromIntersection");
            distanceFromIntersection.DisplayName = "State Variable for Distance from Intersection";
            distanceFromIntersection.Description = "The state to store the distance from the intersection";
            distanceFromIntersection.Required = false;
                                     
            // Example of how to add an element property definition to the step.
            //pd = schema.AddElementProperty("UserElementName", UserElementDefinition.MY_ID);
            //pd.DisplayName = "UserElement Name";
            //pd.Description = "The name of a UserElement element referenced by this step.";
            //pd.Required = true;
        }

        /// <summary>
        /// Method called to create a new instance of this step type to place in a process. 
        /// Returns an instance of the class implementing the IStep interface.
        /// </summary>
        public IStep CreateStep(IPropertyReaders properties)
        {
            return new IntersectionStep(properties);
        }

        #endregion
    }

    class IntersectionStep : IStep
    {
        IPropertyReaders _properties;
        double[] origin = new double[3];
        double[] ray = new double[3];
        double[] box0 = new double[3];
        double[] box1 = new double[3];
        double length = 0;

        double[] steeringVector = new double[3];
        

        public IntersectionStep(IPropertyReaders properties)
        {
            _properties = properties;
        }

        #region IStep Members

        /// <summary>
        /// Method called when a process token executes the step.
        /// </summary>
        public ExitType Execute(IStepExecutionContext context)
        {
                        
            // Example of how to get the value of a step property.
            IPropertyReader myExpressionProp = _properties.GetProperty("MyExpression") as IPropertyReader;
            string myExpressionPropStringValue = myExpressionProp.GetStringValue(context);
            double myExpressionPropDoubleValue = myExpressionProp.GetDoubleValue(context);

            // Example of how to get an element reference specified in an element property of the step.
            IElementProperty myElementProp = (IElementProperty)_properties.GetProperty("UserElementName");
            //UserElement myElement = (UserElement)myElementProp.GetElement(context);

            // Example of how to display a trace line for the step.
            context.ExecutionInformation.TraceInformation(String.Format("The value of expression '{0}' is '{1}'.", myExpressionPropStringValue, myExpressionPropDoubleValue));

            origin[0] = _properties.GetProperty("Ox").GetDoubleValue(context);
            origin[1] = _properties.GetProperty("Oy").GetDoubleValue(context);
            origin[2] = _properties.GetProperty("Oz").GetDoubleValue(context);

            ray[0] = _properties.GetProperty("Rx").GetDoubleValue(context);
            ray[1] = _properties.GetProperty("Ry").GetDoubleValue(context);
            ray[2] = _properties.GetProperty("Rz").GetDoubleValue(context);

            box0[0] = _properties.GetProperty("B0x").GetDoubleValue(context);
            box0[1] = _properties.GetProperty("B0y").GetDoubleValue(context);
            box0[2] = _properties.GetProperty("B0z").GetDoubleValue(context);

            box1[0] = _properties.GetProperty("B1x").GetDoubleValue(context);
            box1[1] = _properties.GetProperty("B1y").GetDoubleValue(context);
            box1[2] = _properties.GetProperty("B1z").GetDoubleValue(context);

            length = _properties.GetProperty("Rlength").GetDoubleValue(context);

            double tmin = 0;
            double tmax = 0;
            double tymin = 0;
            double tymax = 0;
            double tzmin = 0;
            double tzmax = 0;
            
            //check intersection of x bounds of box (planes with a fixed x and varying y/z)
            if (ray[0] != 0)
            {
                tmin = (box0[0] - origin[0]) / ray[0];
                tmax = (box1[1] - origin[0]) / ray[0];
                if (tmin > tmax) Swap<double>(ref tmin, ref tmax);
            }
            else
            {
                tmin = 10 ^ 12;
                tmax = 10 ^ 12;
            }

                    

            //check intersection of y bounds of box (planes with a fixed y and varying x/z)
            if (ray[1] != 0)
            {
                tymin = (box0[1] - origin[1]) / ray[1];
                tymax = (box1[1] - origin[1]) / ray[1];
                if (tymin > tymax) Swap<double>(ref tymin, ref tymax);
            }
            else
            {
                tymin = 10 ^ 12;
                tymax = 10 ^ 12;
            }

            //check that intersects box
            if ((tmin > tymax) || (tymin>tmax))
                return ExitType.AlternateExit;

            //save largest tmin, smallest tmax
            if (tymin > tmin) tmin = tymin;
            if (tymax < tmax) tmax = tymax;

            //check intersection of z bounds of box (planes with a fixed z and varying x/y)
            if (ray[2] != 0)
            {
                tzmin = (box0[2] - origin[2]) / ray[2];
                tzmax = (box1[2] - origin[2]) / ray[2];
                if (tzmin > tzmax) Swap<double>(ref tzmin, ref tzmax);
            }
            else
            {
                tzmin = 10 ^ 12;
                tzmax = 10 ^ 12;
            }
            
            //check for intersection of box
            if (( tmin > tzmax) || (tzmin>tmax))
                return ExitType.AlternateExit;

            //save largest tmin, smallest tmax
            if (tzmin > tmin) tmin = tzmin;
            if (tzmax < tmax) tmax = tzmax;
            
            //if got this far, intersects box.  May also be behind us.
            if (tmin<0 && tmax<0)
                return ExitType.AlternateExit;
            //if tmin is behind us, use only tmax.
            if (tmin<0)
                tmin = tmax;
            
            //at this point tmin represents the shortest distance to an intersection

            //Intersection Point
            double[] IntersectionPoint = new double[3];
            IntersectionPoint[0] = origin[0] + ray[0] * tmin;
            IntersectionPoint[1] = origin[1] + ray[1] * tmin;
            IntersectionPoint[2] = origin[2] + ray[2] * tmin;

            //check if is within line.
            double intersectionLength = Math.Pow(Math.Pow(IntersectionPoint[0]-origin[0],2)  + Math.Pow(IntersectionPoint[1]-origin[1],2) + Math.Pow(IntersectionPoint[2]-origin[2],2),0.5);
            if (length < intersectionLength)
                return ExitType.AlternateExit;

            double raytmax = length/intersectionLength;

            //calculate length of ray within box.  This will be the magnitude of the steering force vector?
            double lengthInBox = length - intersectionLength;

            steeringVector[0] = (IntersectionPoint[0] + ray[0] * (raytmax - tmin));
            steeringVector[1] = (IntersectionPoint[1] + ray[1] * (raytmax - tmin));
            steeringVector[2] = (IntersectionPoint[2] + ray[2] * (raytmax - tmin));
             
            //change direction based on plane
            if (IntersectionPoint[0] == box0[0] || IntersectionPoint[0] == box1[0]) steeringVector[0] = -steeringVector[0];
            else if (IntersectionPoint[1] == box0[1] || IntersectionPoint[1] == box1[1]) steeringVector[1] = -steeringVector[1];
            else if (IntersectionPoint[2] == box0[2] || IntersectionPoint[2] == box1[2]) steeringVector[2] = - steeringVector[2];
            else 
            {
                //throw error
            }
                        
            IStateProperty statepropSFX = (IStateProperty)_properties.GetProperty("SteeringForceX");
            IState stateSFX = statepropSFX.GetState(context);
            stateSFX.StateValue = steeringVector[0];

            IStateProperty statepropSFY = (IStateProperty)_properties.GetProperty("SteeringForceY");
            IState stateSFY = statepropSFY.GetState(context);
            stateSFY.StateValue = steeringVector[1];

            IStateProperty statepropSFZ = (IStateProperty)_properties.GetProperty("SteeringForceZ");
            IState stateSFZ = statepropSFX.GetState(context);
            stateSFZ.StateValue = steeringVector[2];

            IStateProperty statepropDistanceFromIntersection = (IStateProperty)_properties.GetProperty("DistanceFromIntersection");
            IState stateDistanceFromIntersection = statepropDistanceFromIntersection.GetState(context);
            stateDistanceFromIntersection.StateValue = intersectionLength;
            
            return ExitType.FirstExit;                        
        }

        static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
               
        
        #endregion
    }
}
