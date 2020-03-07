using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nasca.Models;

namespace Nasca.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> NodeList()
        {
            ElementDAO dao = new ElementDAO();
            List<Element> elements = dao.selectAll();

            var nodes = new List<Node>();

            var root = new Node
            {
                id = Element.getRoot().getId(),
                parent = "#",
                text = Element.getRoot().getName(),
                type = Element.getRoot().getType(),
                remark = "",
                hasDependency = false
            };

            nodes.Add(root);

            elements.ForEach(element =>
            {
                var node = new Node
                {
                    parent = element.getParent().getId(),
                    parentText = element.getParent().getName(),
                    id = element.getId(),
                    text = element.getName(),
                    type = element.getType(),
                    remark = element.getRemark(),
                    icon = "./images/" + element.getSvgFile() + ".png",
                    hasDependency = (element.getDependency().Count == 0 && element.getDependencyDependOnMe().Count == 0 ? false : true)
                };

                nodes.Add(node);
            });

            return JsonConvert.SerializeObject(nodes);
        }

        [HttpGet]
        public ActionResult<string> NodeType()
        {
            var nodeTypes = new List<NodeType>();
            ElementTypeDAO dao = new ElementTypeDAO();
            List<ElementType> elementTypes = dao.selectAll();

            elementTypes.ForEach(t =>
            {
                var nodeType = new NodeType
                {
                    value = t.getElementType(),
                    text = t.getElementType(),
                    image = "./images/" + t.getSvgFile() + ".png"
                };

                nodeTypes.Add(nodeType);
            });

            return JsonConvert.SerializeObject(nodeTypes);
        }

        [HttpPost]
        public ActionResult<string> DataFlowInfomation([FromForm]string[] nodeStrings)
        {
            Dictionary<Element, AddtionalNodeInfomation> nodes = new Dictionary<Element, AddtionalNodeInfomation>();
            Dictionary<LinkKey, AddtionalLinkInfomation> links = new Dictionary<LinkKey, AddtionalLinkInfomation>();
            List<Element> groups = new List<Element>();

            //1段階目の取得■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
            for (int i = 0; i < nodeStrings.Length; i++)
            {
                //空文字の場合はスキップ
                if (nodeStrings[i].Equals("")) continue;

                //rootノードの場合はスキップ
                if (nodeStrings[i].Equals("root")) continue;

                Element element = Element.getElement(nodeStrings[i]);

                this.putNodeAndLink(nodes, links, element, 1);
                if (!element.isLeaf())
                {
                    foreach (Element child in element.getChild())
                    {
                        this.putNodeAndLink(nodes, links, child, 1);
                    }

                    //昇格のためのgroupエレメントを配列に格納します。
                    //同一Groupの親－子－孫が候補の場合、格納するのは親Groupエレメントのみです。
                    bool isAdd = true;
                    Element unnecesarryGroup = null;
                    foreach (Element group in groups)
                    {
                        if (group.contain(element))
                        {
                            isAdd = false;
                            break;
                        }

                        if (element.contain(group))
                        {
                            unnecesarryGroup = group;
                            break;
                        }
                    }
                    if (isAdd)
                    {
                        groups.Add(element);
                    }
                    if (unnecesarryGroup != null)
                    {
                        groups.Remove(unnecesarryGroup);
                    }
                }
            }

            //2段階目の取得■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

            //2段階目を取得する際に、ループを回すコレクションに対し要素追加を行うとConcurrentModificationExceptionが発生するためコピーします
            Dictionary<Element, AddtionalNodeInfomation> nodes2 = new Dictionary<Element, AddtionalNodeInfomation>();
            nodes2 = new Dictionary<Element, AddtionalNodeInfomation>(nodes);

            //2段階目を取得します
            foreach (KeyValuePair<Element, AddtionalNodeInfomation> nodeSet in nodes)
            {
                this.putNodeAndLink(nodes2, links, nodeSet.Key, 2);
            }

            //グループ化■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
            Dictionary<Element, AddtionalNodeInfomation> nodes3 = new Dictionary<Element, AddtionalNodeInfomation>();
            Dictionary<LinkKey, AddtionalLinkInfomation> links3 = new Dictionary<LinkKey, AddtionalLinkInfomation>();

            foreach (KeyValuePair<Element, AddtionalNodeInfomation> nodeSet in nodes2)
            {
                bool isGroupNode = false;

                foreach (Element group in groups)
                {
                    //グループ内のノード以外を対象とします
                    if (group.contain(nodeSet.Key.getId()))
                    {
                        isGroupNode = true;
                        break;
                    }
                }

                if (!isGroupNode)
                {
                    nodes3.Add(nodeSet.Key, nodeSet.Value);
                }
            }
            foreach (KeyValuePair<LinkKey, AddtionalLinkInfomation> linkSet in links)
            {
                bool isGroupLink = false;
                bool isVirtual = false;
                LinkKey linkKey = null;
                Element source = linkSet.Key.getSource();
                Element target = linkSet.Key.getTarget();

                foreach (Element group in groups)
                {
                    //グループ内のリンク以外を対象とします
                    if (group.contain(linkSet.Key.getSource()) && group.contain(linkSet.Key.getTarget()))
                    {
                        isGroupLink = true;
                        break;
                    }
                    else if (group.contain(linkSet.Key.getSource()))
                    {
                        //グループ内のノードはグループノードに昇格します（ソース側）
                        source = group;
                        isVirtual = true;
                    }
                    else if (group.contain(linkSet.Key.getTarget()))
                    {
                        //グループ内のノードはグループノードに昇格します（ターゲット側）
                        target = group;
                        isVirtual = true;
                    }
                }

                linkKey = new LinkKey(source, target);

                if (!isGroupLink)
                {
                    //昇格した結果、そのリンクがすでに存在していれば集約します。
                    if (links3.ContainsKey(linkKey))
                    {
                        AddtionalLinkInfomation val;
                        links3.TryGetValue(linkKey, out val);
                        this.summarizeLink(linkSet.Value, val, false);
                    }
                    else
                    {
                        linkSet.Value.isVirtual = isVirtual;
                        links3.Add(linkKey, linkSet.Value);
                    }
                }
            }

            //逆向きリンクの集約■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
            Dictionary<LinkKey, AddtionalLinkInfomation> links4 = new Dictionary<LinkKey, AddtionalLinkInfomation>();

            foreach (KeyValuePair<LinkKey, AddtionalLinkInfomation> linkSet3 in links3)
            {
                bool duplex = false;
                foreach (KeyValuePair<LinkKey, AddtionalLinkInfomation> linkSet4 in links4)
                {
                    //ソースとターゲットが同一ノードで逆向きとなっているリンクがあれば集約します
                    if (linkSet3.Key.getSource() == linkSet4.Key.getTarget() && linkSet3.Key.getTarget() == linkSet4.Key.getSource())
                    {
                        this.summarizeLink(linkSet3.Value, linkSet4.Value, true);
                        duplex = true;
                        break;
                    }
                }

                if (!duplex)
                {
                    links4.Add(linkSet3.Key, linkSet3.Value);
                }
            }

            var dataflow = new Dataflow();

            foreach (KeyValuePair<Element, AddtionalNodeInfomation> element in nodes3)
            {
                var node = new Node2
                {
                    parent = element.Key.getParent().getId(),
                    id = element.Key.getId(),
                    name = element.Key.getName(),
                    type = element.Key.getType(),
                    remark = element.Key.getRemark(),
                    svgFile = element.Key.getSvgFile() + ".svg",
                    visible = element.Value.getDistance() == 2 ? false : true,
                    size = element.Key.isLeaf() ? 32 : 64,
                    group = !element.Key.isLeaf(),
                    depth = element.Key.getParent().getId().Equals(Element.getRoot().getId()) ? 0 : element.Key.getParent().getId().Split('.').Length
                };

                dataflow.nodes.Add(node);
            }

            foreach (KeyValuePair<LinkKey, AddtionalLinkInfomation> linkSet in links4)
            {
                Dependency depndency = linkSet.Value.getDependency();

                var link = new Link
                {
                    source = linkSet.Key.getSource().getId(),
                    target = linkSet.Key.getTarget().getId(),
                    isCreate = depndency.dependencyTypeCreate,
                    isRead = depndency.dependencyTypeRead,
                    isUpdate = depndency.dependencyTypeUpdate,
                    isDelete = depndency.dependencyTypeDelete,
                    remark = depndency.remark,
                    io = linkSet.Value.getDirection().ToString(),
                    colorIndex = Convert.ToInt32(depndency.getDependencyType(), 2).ToString(),
                    visible = linkSet.Value.getDistance() == 2 ? false : true,
                    isVirtual = linkSet.Value.isVirtual
                };

                dataflow.links.Add(link);
            }

            return JsonConvert.SerializeObject(dataflow);
        }

        private void putNodeAndLink(Dictionary<Element, AddtionalNodeInfomation> nodes, Dictionary<LinkKey, AddtionalLinkInfomation> links, Element element, int distance)
        {
            //自分自身をノードリストに追加します。
            //結果変数はHashSetなので重複を考慮しなくても本来は問題ないですが、重複した場合後勝ちとなるため、先勝ちとなるよう存在チェックを行います。
            if (!nodes.ContainsKey(element))
            {
                nodes.Add(element, new AddtionalNodeInfomation(distance));
            }

            //依存する要素を取得し結果変数に格納します。
            //結果変数はHashSetなので重複を考慮しなくても本来は問題ないですが、重複した場合後勝ちとなるため、先勝ちとなるよう存在チェックを行います。
            List<Dependency> dependencies = element.getDependency();
            foreach (Dependency dependency in dependencies)
            {
                if (!nodes.ContainsKey(dependency.dependencyElement))
                {
                    nodes.Add(dependency.dependencyElement, new AddtionalNodeInfomation(distance));
                }
                LinkKey linkKey = new LinkKey(element, dependency.dependencyElement);
                if (!links.ContainsKey(linkKey))
                {
                    links.Add(linkKey, new AddtionalLinkInfomation(dependency, distance, this.getDrection(dependency)));
                }
            }

            //依存される要素を取得し結果変数に格納します。
            //結果変数はHashSetなので重複を考慮しなくても本来は問題ないですが、重複した場合後勝ちとなるため、先勝ちとなるよう存在チェックを行います。
            List<Dependency> dependenciesDependOnMe = element.getDependencyDependOnMe();
            foreach (Dependency dependencyDependOnMe in dependenciesDependOnMe)
            {
                if (!nodes.ContainsKey(dependencyDependOnMe.element))
                {
                    nodes.Add(dependencyDependOnMe.element, new AddtionalNodeInfomation(distance));
                }
                LinkKey linkKey = new LinkKey(dependencyDependOnMe.element, element);
                if (!links.ContainsKey(linkKey))
                {
                    links.Add(linkKey, new AddtionalLinkInfomation(dependencyDependOnMe, distance, this.getDrection(dependencyDependOnMe)));
                }
            }
        }

        private void summarizeLink(AddtionalLinkInfomation source, AddtionalLinkInfomation target, bool isReverseDirection)
        {
            target.getDependency().dependencyTypeCreate = target.getDependency().dependencyTypeCreate || source.getDependency().dependencyTypeCreate;
            target.getDependency().dependencyTypeRead = target.getDependency().dependencyTypeRead || source.getDependency().dependencyTypeRead;
            target.getDependency().dependencyTypeUpdate = target.getDependency().dependencyTypeUpdate || source.getDependency().dependencyTypeUpdate;
            target.getDependency().dependencyTypeDelete = target.getDependency().dependencyTypeDelete || source.getDependency().dependencyTypeDelete;
            target.getDependency().remark = target.getDependency().remark + source.getDependency().remark;
            target.isVirtual = true;
            if (isReverseDirection)
            {
                target.addDirection(source.getDirection().reverse());
            }
            else
            {
                target.addDirection(source.getDirection());
            }
        }

        //CRUD情報からIO情報（矢印の方向）
        private Direction getDrection(Dependency dependency)
        {
            Direction result;

            if (dependency.dependencyTypeRead && (dependency.dependencyTypeCreate || dependency.dependencyTypeUpdate || dependency.dependencyTypeDelete))
            {
                result = Direction.IO;
            }
            else if (!dependency.dependencyTypeRead && (dependency.dependencyTypeCreate || dependency.dependencyTypeUpdate || dependency.dependencyTypeDelete))
            {
                result = Direction.O;
            }
            else if (dependency.dependencyTypeRead && !(dependency.dependencyTypeCreate || dependency.dependencyTypeUpdate || dependency.dependencyTypeDelete))
            {
                result = Direction.I;
            }
            else
            {
                result = Direction.None;
            }
            return result;
        }
    }
}