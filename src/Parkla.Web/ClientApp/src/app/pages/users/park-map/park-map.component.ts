import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FullscreenControl, Map, map, NavigationControl } from "@tomtom-international/web-sdk-maps";
import { services } from "@tomtom-international/web-sdk-services";
import SearchBox, { } from "@tomtom-international/web-sdk-plugin-searchbox";
import { ttkey } from '@app/core/constants/private.const';
@Component({
  selector: 'app-park-map',
  templateUrl: './park-map.component.html',
  styleUrls: ['./park-map.component.scss']
})
export class ParkMapComponent implements OnInit, AfterViewInit {

  @ViewChild("appSearchBox")
  appSearchBoxRef!: ElementRef<HTMLElement>;

  appMap?: Map;
  appSearchBox?: SearchBox;
  key = ""

  constructor() { }

  ngOnInit(): void {

  }

  ngAfterViewInit(): void {
    this.loadMap();
    this.appSearchBoxRef.nativeElement
      .appendChild(this.appSearchBox!.getSearchBoxHTML());
  }

  loadMap() {
    this.appMap = map({
      key: ttkey,
      container: "appMap",
      zoom: 12,
      language: "tr-TR",
      center: {
        lat:41.015137,
        lng:28.979530,
      },
    });

    this.appMap.addControl(new NavigationControl());
    this.appMap.addControl(new FullscreenControl());
    this.appMap.showTrafficFlow();

    this.appSearchBox = new SearchBox(services, {
      searchOptions: {
        key: ttkey,
        language: "tr-TR",
        limit: 5
      },
      autocompleteOptions: {
        key: ttkey,
        language: "tr-TR"
      }
    })
  }

}
