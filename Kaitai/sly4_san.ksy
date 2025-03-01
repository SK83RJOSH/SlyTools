meta:
  id: sly4_san
  title: "Sanzaru File"
  application: "Sly Cooper: Thieves in Time"
  file-extension: san san.cooked
  endian: le
  encoding: ascii
seq:
  - id: header
    type: header
  - id: body
    type: body
    size: header.file_size - 4
types:
  header:
    seq:
      - id: magic
        contents: FORM
      - id: file_size
        type: u4
  body:
    seq:
      - id: block
        type: block
        repeat: eos
  block:
    seq:
      - id: type
        type: str
        size: 4
      - id: data
        type:
          switch-on: type
          cases:
            '"PLAT"': plat
            # Segment Types
            '"PRFE"': prfe
            '"PROF"': prof
            '"PRFN"': prfn
            # Texture Types
            '"TEXR"': texr
            '"TXRH"': txrh
            '"TGXT"': tgxt
            '"TINC"': tinc
            # Material Types
            '"MATL"': matl
            '"MTLH"': mtlh
            '"MTLC"': mtlc
            # Mesh Types
            '"MESH"': mesh
            # Geometry Types
            '"GEOB"': geob
            '"GEOH"': geoh
            '"GLOD"': glod
            '"SKEL"': skel
            '"SKHD"': skhd
            '"BONS"': bons
            # Light Types
            # Emitter Types
            _: fill
  plat:
    seq:
      - id: data
        size: 8
  prof:
    seq:
      - id: unknown
        type: u4
  prfe:
    seq:
      - id: unknown
        type: u4
  prfn: # Name block?
    seq:
      - id: size
        type: u4
      - id: name
        type: strz
        size: size - 4
  texr: # Texture type...?
    seq:
      - id: unknown
        type: u4
  txrh: # Texture header?
    seq:
      - id: size
        type: u4
      - id: unknown_0
        type: u1
      - id: name_hash
        type: u4
      - id: unknown_1
        type: u1
        repeat: expr
        repeat-expr: 7
      - id: name
        type: str
        size: 40
  tgxt: # Texture GXT format
    seq:
      - id: size
        type: u4
      - id: data
        size: size - 4
  tinc: # Texture Include
    seq:
      - id: size
        type: u2
      - id: string_length
        type: u4be # This was definitely a mistake on their end... lol
      - id: string
        type: str
        size: string_length
      - id: padding
        size: size - 2 - string_length - 4
  matl:
    seq:
      - id: unknown
        type: u4
  mtlh:
    seq:
      - id: size
        type: u4
      - id: unknown
        type: u1
        repeat: expr
        repeat-expr: size - 4
  mtlc:
    seq:
      - id: size
        type: u4
      - id: unknown
        type: u1
        repeat: expr
        repeat-expr: size - 4
  mesh:
    seq:
      - id: size
        type: u4
      - id: unknown
        type: u1
        repeat: expr
        repeat-expr: size - 4
  geob:
    seq:
      - id: unknown
        type: u4
  geoh:
    seq:
      - id: size
        type: u4
      - id: unknown
        size: size - 4
  glod:
    seq:
      - id: size
        type: u4
      - id: unknown
        size: size - 4
  skel:
    seq:
      - id: unknown
        type: u4
  skhd:
    seq:
      - id: size
        type: u4
      - id: unknown
        size: size - 4
  bons:
    seq:
      - id: size
        type: u4
      - id: unknown
        size: size - 4
  fill:
    seq:
      - id: data
        size: _io.size - _io.pos