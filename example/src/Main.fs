module Main

open Elmish
open Fable.Core
open ReactNativeHelpers
open Fable.Import.ReactNative
open Fable.Helpers.React
open ReactNativeHelpers.Props
// open ReactNativeHelpers.Alert
module R = ReactNativeHelpers

type Model =
  { defaultImage: IImageSourceProperties list
    filteredImages: FilteredImage.Model list
    imageSelectIsVisible: bool }

type Message =
  | ChangeAllImages
  | AddFilteredImage
  | FilteredImageMessage of int * FilteredImage.Message
  | ShowImageSelect
  | HideImageSelect

let init () =
  { defaultImage = (localImage "${entryDir}/../parrot.png")
    filteredImages = []
    imageSelectIsVisible = false },
  Cmd.none

let update (message: Message) model =
  match message with
  | AddFilteredImage ->
    { model with filteredImages = FilteredImage.init(model.defaultImage) :: model.filteredImages }, []

  | ChangeAllImages ->
    // alert("Model", string model, [])
    model, []

  | FilteredImageMessage (id, msg) ->
    { model with filteredImages =
                   model.filteredImages
                   |> List.mapi (fun i m -> if i = id then FilteredImage.update msg m else m) }, []

  | ShowImageSelect ->
    { model with imageSelectIsVisible = true }, []

  | HideImageSelect ->
    { model with imageSelectIsVisible = false }, []

let inline containerStyle<'a> =
  ViewProperties.Style
    [ Padding (Absolute 20.)
      Flex 1. ]

let inline gapStyle<'a> =
  ViewProperties.Style
    [ Height (Absolute 5.) ]

[<Pojo>]
type ScrollToConfig = { y: float }

let view model (dispatch: Dispatch<Message>) =
  let filteredImageDispatch i msg = dispatch <| FilteredImageMessage (i, msg)

  let items = model.filteredImages
              |> List.mapi (fun i c -> FilteredImage.view c (filteredImageDispatch i))

  let mutable scrollView: Option<ScrollView> = None

  let scrollToBottom = fun _ height ->
    match scrollView with
    | Some ref -> ref.scrollTo(U2.Case2 ({ y = height } :> obj))
    | None -> ()

  R.scrollView
    [ ScrollViewProperties.OnContentSizeChange scrollToBottom
      ScrollViewProperties.Ref (fun ref -> scrollView <- Some ref)]
    [ R.view
        [ containerStyle ]
        [ R.button
            [ ButtonProperties.Title "Change all images"
              ButtonProperties.OnPress (fun () -> dispatch ShowImageSelect)]
            [] 
          R.view [ gapStyle ] []
          fragment [] [ yield! items ]
          R.view [ gapStyle ] []
          R.button
            [ ButtonProperties.Title "Add filtered image"
              ButtonProperties.OnPress (fun () -> dispatch AddFilteredImage)]
            [] ] ]