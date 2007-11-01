//XMPP .NET Library Copyright (C) 2006 Dieter Lunn
//
//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 3 of the License, or (at your option)
//any later version.//
//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//
//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections;
using System.Xml;
using System.Reflection;

using xmpp.common;

using log4net;
using log4net.Config;

namespace xmpp.registries
{
    /// <remarks>
    /// TagRegistry stores all the construction information for the <seealso cref="Tag">Tags</seealso> the library is aware of.
    /// </remarks>
	public sealed class TagRegistry : Registry<TagRegistry, RegistryAllocator<TagRegistry>>
	{
        private static readonly ILog logger = LogManager.GetLogger(typeof(TagRegistry));

		private TagRegistry()
		{
            XmlConfigurator.Configure();
		}

		private void AddTag(string localName, string ns, Type t)
		{
			AddTag(new XmlQualifiedName(localName, ns), t);
		}

		private void AddTag(XmlQualifiedName qname, Type t)
		{
			Type[] constructorTypes = new Type[] {
				typeof(string),
				typeof(XmlQualifiedName),
				typeof(XmlDocument)
			};
			ConstructorInfo ci = t.GetConstructor(constructorTypes);
			_registeredItems.Add(qname, ci);
		}

		/// <summary>
		/// Used to add <seealso cref="Tag">Tag(s)</seealso> to the registry.  Using attributes the <see cref="TagRegistry"/> looks for and adds any appropriate tags found in the assembly.
		/// </summary>
		/// <param name="ass">The assembly to search for tags</param>
		public void AddAssembly(Assembly ass)
		{
            logger.Debug("Adding assembly " + ass.FullName);
            //Type[] types = ass.GetTypes();

            /*foreach (Type type in types)
            {
                XmppTagAttribute[] tags = (XmppTagAttribute[])type.GetCustomAttributes(typeof(XmppTagAttribute), false);
                foreach (XmppTagAttribute tag in tags)
                {
                    logger.Debug("Adding: " + type.FullName);
                    AddTag(tag.Prefix, tag.NS, tag.ClassType);
                }
            }*/
            
            XmppTagAttribute[] tags = GetAttributes<XmppTagAttribute>(ass);
            foreach (XmppTagAttribute tag in tags)
            {
            	logger.Debug("Adding: " + tag.Prefix);
            	AddTag(tag.Prefix, tag.NS, tag.ClassType);
            }
		}

        /// <summary>
        /// Creates a new instance of the wanted tag.
        /// </summary>
        /// <param name="prefix">The tag prefix</param>
        /// <param name="qname">Qualified Namespace</param>
        /// <param name="doc">XmlDocument to create tag with</param>
        /// <returns>A new instance of the requested tag</returns>
		public xmpp.common.Tag GetTag(string prefix, XmlQualifiedName qname, XmlDocument doc)
        {
        	ConstructorInfo ci = (ConstructorInfo)_registeredItems[qname];
        	if (ci != null) return (xmpp.common.Tag)ci.Invoke(new object[] { prefix, qname, doc });
        	return null;
        }
	}
}
