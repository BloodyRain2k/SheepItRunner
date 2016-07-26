using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace BloodyRain2k {
	public class DirectDictionary<TK, TV> : Dictionary<TK, TV> {
		private bool _readonly;
		
		public new void Add(TK Key, TV Value) {
			base.Add(Key, Value);
		}

		public void AddIfNotExist(TK Key, TV Value) {
			if (!this.ContainsKey(Key)) { this[Key] = Value; }
		}

		public DirectDictionary(bool ReadOnly = false) {
			_readonly = ReadOnly;
		}

		public DirectDictionary(Dictionary<TK, TV> Dictionary, bool ReadOnly = false) {
			if (Dictionary == null) { return; }
			_readonly = ReadOnly;
			foreach (KeyValuePair<TK, TV> kv in Dictionary) {
				this[kv.Key] = kv.Value;
			}
		}

		public DirectDictionary(object Dictionary, bool ReadOnly = false) {
			if (Dictionary == null) { return; }
			_readonly = ReadOnly;
			foreach (KeyValuePair<TK, TV> kv in (Dictionary<TK, TV>)Dictionary) {
				this[kv.Key] = kv.Value;
			}
		}

		public new TV this[TK Key] {
			set {
				if (_readonly) { throw new Exception(this.GetType() + " is set to readonly"); }
				if (!base.ContainsKey(Key)) {
					base.Add(Key, value);
				} else {
					base[Key] = value;
				}
			}
			get {
				if (!base.ContainsKey(Key)) {
					return default(TV);
				} else {
					return base[Key];
				}
			}

		}
	}

	public class AppSettings : DirectDictionary<string, object> {
		public string Filename = "";
		public readonly DirectDictionary<string, int> Int = new DirectDictionary<string, int>(true);
		public readonly DirectDictionary<string, bool> Bool = new DirectDictionary<string, bool>(true);
		public readonly DirectDictionary<string, float> Float = new DirectDictionary<string, float>(true);
		public readonly DirectDictionary<string, string> String = new DirectDictionary<string, string>(true);

		public new object this[string Key] {
			set {
				string type = value.GetType().ToString();
				type = type.Substring(type.IndexOf(".") + 1);
				if (!base.ContainsKey(Key)) {
					base.Add(Key, value);
				} else {
					base[Key] = value;
				}
				Int.Remove(Key);
				Bool.Remove(Key);
				Float.Remove(Key);
				String.Remove(Key);
				switch (type) {
						case "Float": Float.Add(Key, float.Parse(value.ToString())); break;
						case "String": String.Add(Key, value.ToString()); break;
						case "Boolean": Bool.Add(Key, bool.Parse(value.ToString())); break;
						case "Integer": Int.Add(Key, int.Parse(value.ToString())); break;
				}
			}
			get {
				/*if (!base.ContainsKey(Key)) {
					//throw new KeyNotFoundException();
					return null;
				} else {/**/
				return base[Key];
				//}
			}
		}

		public AppSettings() {
			this.Filename = System.Windows.Forms.Application.StartupPath + @"\" + System.Windows.Forms.Application.ProductName + @"Settings.xml";
			Load();
		}

		public AppSettings(string Filename) {
			this.Filename = Filename;
			Load();
		}

		public string SerializeList(List<string> List) {
			System.Xml.XmlDocument XML = new System.Xml.XmlDocument();
			XmlNode root = XML.CreateElement("List_String");
			XmlNode node;
			XML.AppendChild(root);
			foreach (string s in List) {
				node = XML.CreateElement("String");
				node.InnerText = s;
				root.AppendChild(node);
			}
			return root.InnerXml;
		}

		public string SerializeDictionary(Dictionary<string, string> Dictionary) {
			System.Xml.XmlDocument XML = new System.Xml.XmlDocument();
			XmlNode root = XML.CreateElement("Dictionary_String_String");
			XmlNode node; XmlNode subnode;
			XML.AppendChild(root);
			foreach (KeyValuePair<string, string> kv in Dictionary) {
				node = XML.CreateElement("KeyValuePair");
				node.Attributes.Append(XML.CreateAttribute("Key")).Value = "String";
				node.Attributes.Append(XML.CreateAttribute("Value")).Value = "String";
				subnode = XML.CreateElement("Key");
				subnode.InnerText = kv.Key;
				node.AppendChild(subnode);
				subnode = XML.CreateElement("Value");
				subnode.InnerText = kv.Value;
				node.AppendChild(subnode);
				root.AppendChild(node);
			}
			return root.InnerXml;
		}

		public string Serialize(object Key, object Value) {
			System.Xml.XmlDocument XML = new System.Xml.XmlDocument();
			string type = Value.GetType().ToString(); string subtype = "";
			if (type.Contains("`")) {
				string[] st = type.Substring(type.IndexOf("`") + 2).Replace("[", "").Replace("]", "").Split(',');
				for (int i = 0; i < st.Length; i++) { st[i] = st[i].Substring(st[i].LastIndexOf(".") + 1); }
				subtype = string.Join(",", st);
				type = type.Remove(type.IndexOf("`"));
			}
			type = type.Substring(type.LastIndexOf(".") + 1);
			switch (type) {
				case "List":
				case "Float":
				case "String":
				case "Integer":
				case "Boolean":
					case "Dictionary": break;
					case "DirectDictionary": type = "Dictionary"; break;
					case "Int32": type = "Integer"; break;
					default: throw new Exception("UnknownType");
			}
			XmlNode root = XML.CreateElement(type);
			//root.Attributes.Append(XML.CreateAttribute("Type")).Value = subtype;
			XML.AppendChild(root);
			if (Key == null || Key == "") {
				switch (type) {
						default: root.InnerText = Value.ToString(); break;
				}
			} else {
				root.Attributes.Append(XML.CreateAttribute("Key")).Value = Key.ToString();
				switch (type) {
					case "List":
						root.Attributes.Append(XML.CreateAttribute("Type")).Value = subtype;
						switch (subtype) {
								case "Float": foreach (object o in (List<float>)Value) { root.InnerXml += Serialize(null, o); } break;
								case "String": foreach (object o in (List<string>)Value) { root.InnerXml += Serialize(null, o); } break;
								case "Boolean": foreach (object o in (List<bool>)Value) { root.InnerXml += Serialize(null, o); } break;
								case "Integer": foreach (object o in (List<int>)Value) { root.InnerXml += Serialize(null, o); } break;
								default: throw new Exception("UnknownSubType");
						} break;
					case "Dictionary":
						root.Attributes.Append(XML.CreateAttribute("Type")).Value = subtype;
						switch (subtype) {
								case "String,String": foreach (KeyValuePair<string, string> kv in (Dictionary<string, string>)Value) {
									root.InnerXml += Serialize(kv.Key, kv.Value);
								} break;
								default: throw new Exception("UnknownSubType");
						} break;
					default: /*node = XML.CreateElement("Key"); node.InnerText = Key.ToString(); root.AppendChild(node);
						node = XML.CreateElement("Value"); node.InnerText = Value.ToString(); root.AppendChild(node); break; /**/
						root.InnerText = Value.ToString(); break;
				}
			}
			return XML.InnerXml;
		}

		public DirectDictionary<string, string> DeserializeDDictStrStr(XmlNode Node) {
			DirectDictionary<string, string> result = new DirectDictionary<string, string>();
			foreach (XmlNode n in Node.ChildNodes) {
				result[n.Attributes["Key"].Value.ToString()] = DeserializeStr(n);
			} return result;
		}

		public List<string> DeserializeListStr(XmlNode Node) {
			List<string> result = new List<string>();
			foreach (XmlNode n in Node.ChildNodes) {
				result.Add(DeserializeStr(n));
			} return result;
		}

		public string DeserializeStr(XmlNode Node) { return Node.InnerText.ToString(); }
		public float DeserializeFloat(XmlNode Node) { return float.Parse(Node.InnerText.ToString()); }
		public bool DeserializeBool(XmlNode Node) { return bool.Parse(Node.InnerText.ToString()); }
		public int DeserializeInt(XmlNode Node) { return int.Parse(Node.InnerText.ToString()); }

		public bool Load() {
			var success = true;
			if (System.IO.File.Exists(Filename)) {
				System.Xml.XmlDocument XML = new System.Xml.XmlDocument();
				try { XML.Load(Filename); } catch (Exception ex) { return false; }
				foreach (System.Xml.XmlNode n in XML.ChildNodes[0].ChildNodes) {
					try {
						string type = n.Name; string key = ""; string subtype;
						if (n.Attributes.GetNamedItem("Key") != null) { key = n.Attributes["Key"].Value.ToString(); }
						switch (type) {
							case "Integer":
								Int.Add(key, DeserializeInt(n)); break;
							case "Boolean":
								Bool.Add(key, DeserializeBool(n)); break;
							case "String":
								String.Add(key, DeserializeStr(n)); break;
							case "Float":
								Float.Add(key, DeserializeFloat(n)); break;
							case "List":
								subtype = n.Attributes["Type"].Value.ToString();
								switch (subtype) {
										case "String": this[key] = DeserializeListStr(n); break;
										default: throw new Exception("UnknownSubType");
								} break;
							case "Dictionary":
								subtype = n.Attributes["Type"].Value.ToString();
								switch (subtype) {
										case "String,String": this[key] = DeserializeDDictStrStr(n); break;
										default: throw new Exception("UnknownSubType");
								} break;
							default:
								throw new Exception("UnknownType");
						}
					} catch (Exception ex) {
						System.Diagnostics.Debug.Print(ex.StackTrace.Split('\n')[0] + ": " + ex.Message);
						success = false;
					}
				}
			}
			return success;
		}

		public bool Save() {
			var success = true;
			System.Xml.XmlDocument XML = new System.Xml.XmlDocument();
			XmlNode root = XML.CreateElement("Settings");
			XML.AppendChild(root);
			
			foreach (KeyValuePair<string, object> kv in this) {
				try {
					root.InnerXml += Serialize(kv.Key.ToString(), kv.Value);
				} catch (Exception ex) {
					System.Diagnostics.Debug.Print(ex.StackTrace.Split('\n')[0] + ": " + ex.Message);
					success = false;
				}
			}
			
			XML.Save(Filename);
			return success;
		}
	}
}
