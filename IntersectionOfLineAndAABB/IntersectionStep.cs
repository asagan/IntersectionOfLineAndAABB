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
            IPropertyDefinition Ox, Oy, Oz;
            IPropertyDefinition Rx, Ry, Rz;
            IPropertyDefinition B0x, B0y, B0z;
            IPropertyDefinition B1x, B1y, B1z;

            Ox = schema.AddExpressionProperty("Ox", "0.0");
            Oy = schema.AddExpressionProperty("Oy", "0.0");
            Oz = schema.AddExpressionProperty("Oz", "0.0");
            Rx = schema.AddExpressionProperty("Rx", "1.0");
            Ry = schema.AddExpressionProperty("Ry", "1.0");
            Rz = schema.AddExpressionProperty("Rz", "1.0");

            Ox.DisplayName = "Line Origin X";
            Oy.DisplayName = "Line Origin Y";
            Oz.DisplayName = "Line Origin Z";
            Rx.DisplayName = "Line X Vector";
            Ry.DisplayName = "Line Y Vector";
            Rz.DisplayName = "Line Z Vector";

            Ox.Description = "The X coordinate of the beginning of the line";
            Oy.Description = "The Y coordinate of the beginning of the line";
            Oz.Description = "The Z coordinate of the beginning of the line";
            Rx.Description = "The X component of the vector describing the lines direction";
            Ry.Description = "The Y component of the vector describing the lines direction";
            Rz.Description = "The Z component of the vector describing the lines direction";

            Ox.Required = true;
            Oy.Required = true;
            Oz.Required = true;
            Rx.Required = true;
            Ry.Required = true;
            Rz.Required = true;

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
            UserElement myElement = (UserElement)myElementProp.GetElement(context);

            // Example of how to display a trace line for the step.
            context.ExecutionInformation.TraceInformation(String.Format("The value of expression '{0}' is '{1}'.", myExpressionPropStringValue, myExpressionPropDoubleValue));

            double 



            if (intersects)
                return ExitType.FirstExit;
            else
                return ExitType.AlternateExit;
    
            
        }

        #endregion
    }
}
