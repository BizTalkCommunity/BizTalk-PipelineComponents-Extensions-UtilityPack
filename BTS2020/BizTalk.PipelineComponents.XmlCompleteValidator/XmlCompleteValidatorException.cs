using System;
using System.Runtime.Serialization;
using Microsoft.BizTalk.Component;

namespace BizTalk.PipelineComponents.XmlCompleteValidator
{
	/// <summary>
	/// defines the type of error this exception represents
	/// </summary>
	public enum ExceptionType
	{
		/// <summary>
		/// a duplicate namespace was encountered
		/// </summary>
		DUPLICATE_NAMESPACE = 0,
		/// <summary>
		/// the component was unable to retrieve the document spec by name
		/// </summary>
		CANNOT_GET_DOCSPEC_BY_NAME,
		/// <summary>
		/// the component was unable to retrieve the document spec by type
		/// </summary>
		CANNOT_GET_DOCSPEC_BY_TYPE,
		/// <summary>
		/// the validation of the inbound message failed
		/// </summary>
		VALIDATOR_FAILED,
		/// <summary>
		/// the component was unable to retrieve a specified schema
		/// </summary>
		NOSCHEMA,
	}

	/// <summary>
	/// exception specific to this component indicating validation failed
	/// </summary>
    [Serializable]
    public class XmlCompleteValidatorException : ComponentException
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="description"></param>
        public XmlCompleteValidatorException(ExceptionType type, string description) : this(type, description, (string[]) null)
        {
        }

		/// <summary>
		/// serialization specific constructor
		/// </summary>
		/// <param name="serializationInfo"></param>
		/// <param name="streamingContext"></param>
        protected XmlCompleteValidatorException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		/// <param name="description"></param>
		/// <param name="args"></param>
        public XmlCompleteValidatorException(ExceptionType type, string description, params string[] args) : base((int) type, description, args)
        {
			throw new Exception(args[0]);
        }
    } 
}

