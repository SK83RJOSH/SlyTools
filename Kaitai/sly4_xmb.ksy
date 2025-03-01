meta:
  id: sly4_xmb
  title: "XML Binary"
  application: "Sly Cooper: Thieves in Time"
  file-extension: xmb
  endian: le
  encoding: ASCII
seq:
  - id: header
    type: header
  - id: nodes
    type: array("node", header.node_count)
types:
  header:
    seq:
      - id: magic
        contents: 'XMLB'
      - id: version
        type: u2
      - id: unknown
        type: u2
      - id: platform
        size: 4
        type: str
      - id: file_size
        type: s4
      - id: node_count
        type: u2
      - id: padding
        size: 2
  raw_string:
    seq:
      - id: value
        type: strz
  hash_string:
    seq:
      - id: hash
        type: u4
      - id: value
        type: strz
  node:
    seq:
      - id: name_hash
        type: u4
      - id: value
        type: pointer("raw_string")
      - id: value_hash
        type: pointer("hash_string")
      - id: node_count
        type: u2
      - id: attribute_count
        type: u2
      - id: nodes
        type: pointer_array("node", node_count)
      - id: attributes
        type: attributes(attribute_count)
      - id: attribute_types
        type: u4
        repeat: expr
        repeat-expr: ((attribute_count / 8) + 1) >> 0
  attribute:
    params:
      - id: index
        type: s2
    seq:
        - id: name_hash
          type: u4
        - id: data_offset
          type: u4
    instances:
      instance:
        pos: data_offset
        type:
          switch-on: ((_parent._parent.attribute_types[index >> 3]) >> (4 * (index & 7))) & 0xF
          cases:
            0: hash_string
            1: s4
            2: f4
            3: vector2_f4
            4: vector3_f4
            5: vector4_u1
            6: u4
  attributes:
    params:
      - id: count
        type: u4
    seq:
      - id: offset
        type: u4
    instances:
      instance:
        pos: offset
        type: attribute(_index)
        repeat: expr
        repeat-expr: count
  pointer:
    params:
      - id: type
        type: str
    seq:
      - id: offset
        type: u4
    instances:
      instance:
        pos: offset
        type:
          switch-on: type
          cases:
            '"node"': node
            '"raw_string"': raw_string
            '"hash_string"': hash_string
  pointer_array:
    params:
      - id: type
        type: str
      - id: count
        type: u2
    seq:
      - id: offset
        type: u4
    instances:
      instance:
        pos: offset
        type: pointer(type)
        repeat: expr
        repeat-expr: count
  vector2_f4:
    seq:
      - id: x
        type: f4
      - id: y
        type: f4
  vector3_f4:
    seq:
      - id: x
        type: f4
      - id: y
        type: f4
      - id: z
        type: f4
  vector4_u1:
    seq:
      - id: r
        type: u1
      - id: g
        type: u1
      - id: b
        type: u1
      - id: a
        type: u1
  array:
    params:
      - id: type
        type: str
      - id: count
        type: u2
    seq:
      - id: offset
        type: u4
    instances:
      instance:
        pos: offset
        type:
          switch-on: type
          cases:
            '"node"': node
            '"hash_string"': hash_string
        repeat: expr
        repeat-expr: count