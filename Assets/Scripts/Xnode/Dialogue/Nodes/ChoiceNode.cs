using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using XNode;

namespace Xnode.Dialogue.Nodes
{
    public class ChoiceNode : InNode
    {
        [Output(dynamicPortList = true, connectionType = ConnectionType.Override)] [ReadOnly]
        public List<Connection> outputs;

        /// <summary>
        ///     选项列表
        /// </summary>
        public List<Sprite> choices;

        private void OnValidate()
        {
            for (var i = outputs.Count; i < choices.Count; i++)
                outputs.Add(new Connection());
            for (var i = choices.Count; i < outputs.Count; i++)
                outputs.Remove(outputs.LastOrDefault());

            UpdatePorts();
            VerifyConnections();
        }


        public override object GetValue(NodePort port)
        {
            if (port.fieldName.Equals(nameof(outputs))) return this;
            return base.GetValue(port);
        }
    }
}