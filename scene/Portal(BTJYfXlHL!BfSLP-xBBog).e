13
128849018881
365682631791051 1716483885753440800
{
  "name": "Portal",
  "local_enabled": true,
  "local_position": {
    "X": 0.2610533237457275,
    "Y": 22.3266296386718750
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 1,
    "Y": 1
  },
  "previous_sibling": "11198096959298:1716403445063905600",
  "next_sibling": "17080452178471:1721077611249895700",
  "parent": "3995215179226:1716399837848180900",
  "spawn_as_networked_entity": true
},
{
  "cid": 1,
  "aoid": "365682631954311:1716483885753517900",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "environment/portal/cosmicPortal.spine",
    "ordered_skins": [

    ],
    "depth_offset": 1.6399999856948853,
    "skeleton_scale": {
      "X": 2.4000000953674316,
      "Y": 2.4000000953674316
    },
    "mask_in_shadow": false
  }
},
{
  "cid": 3,
  "aoid": "365682632059222:1716483885753567700",
  "component_type": "Internal_Component",
  "internal_component_type": "Interactable",
  "data": {
    "prompt_offset": {
      "X": 0,
      "Y": 1
    },
    "text": "Teleport to the Hub",
    "hold_text": "",
    "radius": 2,
    "required_hold_time": 0.6000000238418579
  }
},
{
  "cid": 2,
  "aoid": "366166482724117:1716484114849628600",
  "component_type": "Mono_Component",
  "mono_component_type": "ZoneTeleporter",
  "data": {
    "ChangeStatusTo": "Safe"
  }
}
